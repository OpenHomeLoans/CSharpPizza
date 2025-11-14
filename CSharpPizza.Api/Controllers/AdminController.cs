using AutoMapper;
using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;
using CSharpPizza.Domain.Services;
using CSharpPizza.DTO.Admin;
using CSharpPizza.DTO.Orders;
using CSharpPizza.DTO.Pizzas;
using CSharpPizza.DTO.Toppings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpPizza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IPizzaService _pizzaService;
    private readonly IToppingService _toppingService;
    private readonly IRepository<Order> _orderRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IRepository<Topping> _toppingRepository;
    private readonly ILoggingService _loggingService;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IOrderService orderService,
        IPizzaService pizzaService,
        IToppingService toppingService,
        IRepository<Order> orderRepository,
        IPizzaRepository pizzaRepository,
        IRepository<Topping> toppingRepository,
        ILoggingService loggingService,
        IMapper mapper,
        ILogger<AdminController> logger)
    {
        _orderService = orderService;
        _pizzaService = pizzaService;
        _toppingService = toppingService;
        _orderRepository = orderRepository;
        _pizzaRepository = pizzaRepository;
        _toppingRepository = toppingRepository;
        _loggingService = loggingService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders with optional filtering
    /// </summary>
    [HttpGet("orders")]
    [ProducesResponseType(typeof(List<AdminOrderListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<AdminOrderListDto>>> GetAllOrders(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? customerName = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Admin getting all orders with filters - Status: {Status}, StartDate: {StartDate}, EndDate: {EndDate}, CustomerName: {CustomerName}",
            status, startDate, endDate, customerName);

        _ = _loggingService.LogInfoAsync($"Admin retrieved orders list with filters", 
            $"Status: {status}, StartDate: {startDate}, EndDate: {endDate}, CustomerName: {customerName}");

        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        
        // Apply filters
        var filteredOrders = orders.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                filteredOrders = filteredOrders.Where(o => o.Status == orderStatus);
            }
        }

        if (startDate.HasValue)
        {
            filteredOrders = filteredOrders.Where(o => o.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            filteredOrders = filteredOrders.Where(o => o.CreatedAt <= endDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(customerName))
        {
            filteredOrders = filteredOrders.Where(o => 
                o.User.Name.Contains(customerName, StringComparison.OrdinalIgnoreCase));
        }

        var orderList = filteredOrders
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => _mapper.Map<AdminOrderListDto>(o))
            .ToList();

        return Ok(orderList);
    }

    /// <summary>
    /// Get order details by ID
    /// </summary>
    [HttpGet("orders/{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin getting order details for ID: {OrderId}", id);
        
        _ = _loggingService.LogInfoAsync($"Admin retrieved order details", $"OrderId: {id}");

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        
        if (order == null)
        {
            return NotFound(new { error = $"Order with ID {id} not found" });
        }
        
        return Ok(order);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPut("orders/{id}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(
        Guid id, 
        [FromBody] UpdateOrderStatusDto updateDto, 
        CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Admin updating order {OrderId} status to: {Status}", id, updateDto.Status);
        
        _ = _loggingService.LogInfoAsync($"Admin updated order status", 
            $"OrderId: {id}, NewStatus: {updateDto.Status}");

        var order = await _orderService.UpdateOrderStatusAsync(id, updateDto.Status, cancellationToken);
        return Ok(order);
    }

    /// <summary>
    /// Get all pizzas including soft-deleted ones
    /// </summary>
    [HttpGet("pizzas")]
    [ProducesResponseType(typeof(List<PizzaListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PizzaListDto>>> GetAllPizzas(CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Admin getting all pizzas including soft-deleted");
        
        _ = _loggingService.LogInfoAsync("Admin retrieved all pizzas including soft-deleted");

        var pizzas = await _pizzaRepository.GetAllAsync(cancellationToken);
        var pizzaList = pizzas.Select(p =>
        {
            var dto = _mapper.Map<PizzaListDto>(p);
            dto.ComputedCost = _pizzaService.ComputePizzaCost(p);
            return dto;
        }).ToList();

        return Ok(pizzaList);
    }

    /// <summary>
    /// Create a new pizza
    /// </summary>
    [HttpPost("pizzas")]
    [ProducesResponseType(typeof(PizzaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PizzaDto>> CreatePizza(
        [FromBody] CreatePizzaDto createDto, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin creating new pizza: {Name}", createDto.Name);
        
        _ = _loggingService.LogInfoAsync($"Admin created new pizza", $"PizzaName: {createDto.Name}");

        var pizza = await _pizzaService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = pizza.Id }, pizza);
    }

    /// <summary>
    /// Update a pizza
    /// </summary>
    [HttpPut("pizzas/{id}")]
    [ProducesResponseType(typeof(PizzaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PizzaDto>> UpdatePizza(
        Guid id, 
        [FromBody] UpdatePizzaDto updateDto, 
        CancellationToken cancellationToken)
    {
     
        
        _logger.LogInformation("Admin updating pizza with ID: {Id}", id);
        
        _ = _loggingService.LogInfoAsync($"Admin updated pizza", $"PizzaId: {id}, PizzaName: {updateDto.Name}");

        var pizza = await _pizzaService.UpdateAsync(id, updateDto, cancellationToken);
        
        if (pizza == null)
        {
            return NotFound(new { error = $"Pizza with ID {id} not found" });
        }
        
        return Ok(pizza);
    }

    /// <summary>
    /// Soft delete a pizza
    /// </summary>
    [HttpDelete("pizzas/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletePizza(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin soft deleting pizza with ID: {Id}", id);
        
        _ = _loggingService.LogInfoAsync($"Admin soft deleted pizza", $"PizzaId: {id}");

        var result = await _pizzaService.DeleteAsync(id, cancellationToken);
        
        if (!result)
        {
            return NotFound(new { error = $"Pizza with ID {id} not found" });
        }
        
        return NoContent();
    }

    /// <summary>
    /// Get all toppings including soft-deleted ones
    /// </summary>
    [HttpGet("toppings")]
    [ProducesResponseType(typeof(List<ToppingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<ToppingDto>>> GetAllToppings(CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Admin getting all toppings including soft-deleted");
        
        _ = _loggingService.LogInfoAsync("Admin retrieved all toppings including soft-deleted");

        var toppings = await _toppingRepository.GetAllAsync(cancellationToken);
        var toppingList = toppings.Select(t => _mapper.Map<ToppingDto>(t)).ToList();

        return Ok(toppingList);
    }

    /// <summary>
    /// Create a new topping
    /// </summary>
    [HttpPost("toppings")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ToppingDto>> CreateTopping(
        [FromBody] CreateToppingDto createDto, 
        CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Admin creating new topping: {Name}", createDto.Name);
        
        _ = _loggingService.LogInfoAsync($"Admin created new topping", $"ToppingName: {createDto.Name}");

        var topping = await _toppingService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetAllToppings), new { id = topping.Id }, topping);
    }

    /// <summary>
    /// Update a topping
    /// </summary>
    [HttpPut("toppings/{id}")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ToppingDto>> UpdateTopping(
        Guid id, 
        [FromBody] UpdateToppingDto updateDto, 
        CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Admin updating topping with ID: {Id}", id);
        
        _ = _loggingService.LogInfoAsync($"Admin updated topping", $"ToppingId: {id}, ToppingName: {updateDto.Name}");

        var topping = await _toppingService.UpdateAsync(id, updateDto, cancellationToken);
        
        if (topping == null)
        {
            return NotFound(new { error = $"Topping with ID {id} not found" });
        }
        
        return Ok(topping);
    }

    /// <summary>
    /// Soft delete a topping
    /// </summary>
    [HttpDelete("toppings/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTopping(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin soft deleting topping with ID: {Id}", id);
        
        _ = _loggingService.LogInfoAsync($"Admin soft deleted topping", $"ToppingId: {id}");

        var result = await _toppingService.DeleteAsync(id, cancellationToken);
        
        if (!result)
        {
            return NotFound(new { error = $"Topping with ID {id} not found" });
        }
        
        return NoContent();
    }
}