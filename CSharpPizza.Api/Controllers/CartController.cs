using System.Security.Claims;
using CSharpPizza.Domain.Services;
using CSharpPizza.DTO.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpPizza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
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
    /// Get current user's cart
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartDto>> GetCart(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting cart for user: {UserId}", userId);
        var cart = await _cartService.GetCartAsync(userId, cancellationToken);
        return Ok(cart);
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] AddToCartDto addToCartDto, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Adding item to cart for user: {UserId}", userId);
        var cart = await _cartService.AddToCartAsync(userId, addToCartDto, cancellationToken);
        return Ok(cart);
    }

    /// <summary>
    /// Update cart item
    /// </summary>
    [HttpPut("items/{id}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid id, [FromBody] UpdateCartItemDto updateDto, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Updating cart item {ItemId} for user: {UserId}", id, userId);
        var cart = await _cartService.UpdateCartItemAsync(userId, id, updateDto, cancellationToken);
        
        if (cart == null)
        {
            return NotFound(new { error = $"Cart item with ID {id} not found" });
        }
        
        return Ok(cart);
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("items/{id}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Removing cart item {ItemId} for user: {UserId}", id, userId);
        var cart = await _cartService.RemoveFromCartAsync(userId, id, cancellationToken);
        
        if (cart == null)
        {
            return NotFound(new { error = $"Cart item with ID {id} not found" });
        }
        
        return Ok(cart);
    }

    /// <summary>
    /// Clear cart
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Clearing cart for user: {UserId}", userId);
        await _cartService.ClearCartAsync(userId, cancellationToken);
        return NoContent();
    }
}