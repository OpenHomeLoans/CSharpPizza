using System.Text.Json;
using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;
using CSharpPizza.DTO.Orders;

namespace CSharpPizza.Domain.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;

    public OrderService(
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Cart> cartRepository,
        ICartService cartService,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _cartRepository = cartRepository;
        _cartService = cartService;
        _mapper = mapper;
    }

    public async Task<List<OrderListDto>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.FindAsync(o => o.UserId == userId, cancellationToken);
        var orderList = new List<OrderListDto>();

        foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
        {
            var dto = _mapper.Map<OrderListDto>(order);
            dto.ItemCount = order.OrderItems.Count;
            orderList.Add(dto);
        }

        return orderList;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateOrderFromCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Get user's cart
        var carts = await _cartRepository.FindAsync(c => c.UserId == userId, cancellationToken);
        var cart = carts.FirstOrDefault();

        if (cart == null || !cart.CartItems.Any())
        {
            throw new InvalidOperationException("Cart is empty");
        }

        // Calculate total
        var totalAmount = _cartService.ComputeCartTotal(cart);

        // Create order
        var order = new Order
        {
            UserId = userId,
            Status = Data.Entities.OrderStatus.New,
            TotalAmount = totalAmount
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        // Create order items from cart items (snapshot)
        foreach (var cartItem in cart.CartItems)
        {
            // Calculate item price with customizations
            decimal itemPrice = cartItem.Pizza.BasePrice;

            // Build toppings list for snapshot
            var toppingsList = new List<ToppingSnapshot>();

            // Add default pizza toppings
            foreach (var pizzaTopping in cartItem.Pizza.PizzaToppings)
            {
                var isRemoved = cartItem.CartItemToppings.Any(cit => 
                    cit.ToppingId == pizzaTopping.ToppingId && !cit.IsAdded);

                if (!isRemoved)
                {
                    itemPrice += pizzaTopping.Topping?.Cost ?? 0;
                    toppingsList.Add(new ToppingSnapshot
                    {
                        Name = pizzaTopping.Topping?.Name ?? "",
                        Cost = pizzaTopping.Topping?.Cost ?? 0
                    });
                }
            }

            // Add custom added toppings
            foreach (var cartItemTopping in cartItem.CartItemToppings.Where(cit => cit.IsAdded))
            {
                var isDefaultTopping = cartItem.Pizza.PizzaToppings.Any(pt => 
                    pt.ToppingId == cartItemTopping.ToppingId);

                if (!isDefaultTopping)
                {
                    itemPrice += cartItemTopping.Topping?.Cost ?? 0;
                    toppingsList.Add(new ToppingSnapshot
                    {
                        Name = cartItemTopping.Topping?.Name ?? "",
                        Cost = cartItemTopping.Topping?.Cost ?? 0
                    });
                }
            }

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                PizzaName = cartItem.Pizza.Name,
                PizzaPrice = itemPrice,
                Quantity = cartItem.Quantity,
                Toppings = JsonSerializer.Serialize(toppingsList)
            };

            await _orderItemRepository.AddAsync(orderItem, cancellationToken);
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);
        await _orderItemRepository.SaveChangesAsync(cancellationToken);

        // Clear the cart
        await _cartService.ClearCartAsync(userId, cancellationToken);

        // Return the created order
        var createdOrder = await _orderRepository.GetByIdAsync(order.Id, cancellationToken);
        return _mapper.Map<OrderDto>(createdOrder!);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, Data.Entities.OrderStatus status, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
        }

        order.Status = status;
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            return false;
        }

        // Only allow cancellation if order is New or Preparing
        if (order.Status != Data.Entities.OrderStatus.New && order.Status != Data.Entities.OrderStatus.Preparing)
        {
            throw new InvalidOperationException("Cannot cancel order in current status");
        }

        var result = await _orderRepository.SoftDeleteAsync(orderId, cancellationToken);
        if (result)
        {
            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    private class ToppingSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
    }
}