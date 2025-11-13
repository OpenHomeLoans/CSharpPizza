using System.Security.Claims;
using CSharpPizza.Data.Entities;
using CSharpPizza.Domain.Services;
using CSharpPizza.DTO.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpPizza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }
        return userId;
    }

    /// <summary>
    /// Get current user's orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OrderListDto>>> GetOrders(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting orders for user: {UserId}", userId);
        var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order with ID: {OrderId}", id);
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        
        if (order == null)
        {
            return NotFound(new { error = $"Order with ID {id} not found" });
        }
        
        return Ok(order);
    }

    /// <summary>
    /// Create order from cart
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderDto>> CreateOrder(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Creating order from cart for user: {UserId}", userId);
        var order = await _orderService.CreateOrderFromCartAsync(userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Update order status (Admin only)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto updateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating order {OrderId} status to: {Status}", id, updateDto.Status);
        var order = await _orderService.UpdateOrderStatusAsync(id, updateDto.Status, cancellationToken);
        return Ok(order);
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling order: {OrderId}", id);
        var result = await _orderService.CancelOrderAsync(id, cancellationToken);
        
        if (!result)
        {
            return NotFound(new { error = $"Order with ID {id} not found" });
        }
        
        return NoContent();
    }
}