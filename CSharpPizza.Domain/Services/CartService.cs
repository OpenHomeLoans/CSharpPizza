using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;
using CSharpPizza.DTO.Carts;

namespace CSharpPizza.Domain.Services;

public class CartService : ICartService
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IRepository<Topping> _toppingRepository;
    private readonly IPizzaService _pizzaService;
    private readonly IMapper _mapper;

    public CartService(
        IRepository<Cart> cartRepository,
        IRepository<CartItem> cartItemRepository,
        IPizzaRepository pizzaRepository,
        IRepository<Topping> toppingRepository,
        IPizzaService pizzaService,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _pizzaRepository = pizzaRepository;
        _toppingRepository = toppingRepository;
        _pizzaService = pizzaService;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var cartDto = _mapper.Map<CartDto>(cart);
        cartDto.TotalAmount = ComputeCartTotal(cart);
        return cartDto;
    }

    public async Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        
        // Verify pizza exists
        var pizza = await _pizzaRepository.GetByIdAsync(addToCartDto.PizzaId, cancellationToken);
        if (pizza == null)
        {
            throw new KeyNotFoundException($"Pizza with ID {addToCartDto.PizzaId} not found");
        }

        // Create cart item
        var cartItem = new CartItem
        {
            CartId = cart.Id,
            PizzaId = addToCartDto.PizzaId,
            Quantity = addToCartDto.Quantity
        };

        // Add custom toppings
        foreach (var toppingId in addToCartDto.AddedToppingIds)
        {
            var topping = await _toppingRepository.GetByIdAsync(toppingId, cancellationToken);
            if (topping != null)
            {
                cartItem.CartItemToppings.Add(new CartItemTopping
                {
                    CartItemId = cartItem.Id,
                    ToppingId = toppingId,
                    IsAdded = true
                });
            }
        }

        // Remove toppings
        foreach (var toppingId in addToCartDto.RemovedToppingIds)
        {
            var topping = await _toppingRepository.GetByIdAsync(toppingId, cancellationToken);
            if (topping != null)
            {
                cartItem.CartItemToppings.Add(new CartItemTopping
                {
                    CartItemId = cartItem.Id,
                    ToppingId = toppingId,
                    IsAdded = false
                });
            }
        }

        await _cartItemRepository.AddAsync(cartItem, cancellationToken);
        await _cartItemRepository.SaveChangesAsync(cancellationToken);

        // Reload cart with items
        cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var cartDto = _mapper.Map<CartDto>(cart);
        cartDto.TotalAmount = ComputeCartTotal(cart);
        return cartDto;
    }

    public async Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto updateDto, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        
        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
        {
            throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found");
        }

        // Update quantity
        cartItem.Quantity = updateDto.Quantity;

        // Clear existing topping customizations
        cartItem.CartItemToppings.Clear();

        // Add new topping customizations
        foreach (var toppingId in updateDto.AddedToppingIds)
        {
            var topping = await _toppingRepository.GetByIdAsync(toppingId, cancellationToken);
            if (topping != null)
            {
                cartItem.CartItemToppings.Add(new CartItemTopping
                {
                    CartItemId = cartItem.Id,
                    ToppingId = toppingId,
                    IsAdded = true
                });
            }
        }

        foreach (var toppingId in updateDto.RemovedToppingIds)
        {
            var topping = await _toppingRepository.GetByIdAsync(toppingId, cancellationToken);
            if (topping != null)
            {
                cartItem.CartItemToppings.Add(new CartItemTopping
                {
                    CartItemId = cartItem.Id,
                    ToppingId = toppingId,
                    IsAdded = false
                });
            }
        }

        await _cartItemRepository.UpdateAsync(cartItem, cancellationToken);
        await _cartItemRepository.SaveChangesAsync(cancellationToken);

        // Reload cart
        cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var cartDto = _mapper.Map<CartDto>(cart);
        cartDto.TotalAmount = ComputeCartTotal(cart);
        return cartDto;
    }

    public async Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        
        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
        {
            throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found");
        }

        await _cartItemRepository.DeleteAsync(cartItemId, cancellationToken);
        await _cartItemRepository.SaveChangesAsync(cancellationToken);

        // Reload cart
        cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var cartDto = _mapper.Map<CartDto>(cart);
        cartDto.TotalAmount = ComputeCartTotal(cart);
        return cartDto;
    }

    public async Task<bool> ClearCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var carts = await _cartRepository.FindAsync(c => c.UserId == userId, cancellationToken);
        var cart = carts.FirstOrDefault();
        
        if (cart == null)
        {
            return false;
        }

        // Delete all cart items
        foreach (var item in cart.CartItems.ToList())
        {
            await _cartItemRepository.DeleteAsync(item.Id, cancellationToken);
        }

        await _cartItemRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public decimal ComputeCartTotal(Cart cart)
    {
        decimal total = 0;

        foreach (var cartItem in cart.CartItems)
        {
            // Start with pizza base price
            decimal itemCost = cartItem.Pizza.BasePrice;

            // Add default pizza toppings cost
            foreach (var pizzaTopping in cartItem.Pizza.PizzaToppings)
            {
                // Check if this topping is removed
                var isRemoved = cartItem.CartItemToppings.Any(cit => 
                    cit.ToppingId == pizzaTopping.ToppingId && !cit.IsAdded);
                
                if (!isRemoved)
                {
                    itemCost += pizzaTopping.Topping?.Cost ?? 0;
                }
            }

            // Add custom added toppings cost
            foreach (var cartItemTopping in cartItem.CartItemToppings.Where(cit => cit.IsAdded))
            {
                // Only add if it's not already in pizza's default toppings
                var isDefaultTopping = cartItem.Pizza.PizzaToppings.Any(pt => 
                    pt.ToppingId == cartItemTopping.ToppingId);
                
                if (!isDefaultTopping)
                {
                    itemCost += cartItemTopping.Topping?.Cost ?? 0;
                }
            }

            total += itemCost * cartItem.Quantity;
        }

        return total;
    }

    private async Task<Cart> GetOrCreateCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var carts = await _cartRepository.FindAsync(c => c.UserId == userId, cancellationToken);
        var cart = carts.FirstOrDefault();

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            await _cartRepository.AddAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);
        }

        return cart;
    }
}