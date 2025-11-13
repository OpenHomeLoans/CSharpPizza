using System.Security.Claims;
using CSharpPizza.Domain.Services;
using CSharpPizza.DTO.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpPizza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
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
    /// Get current user
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting current user: {UserId}", userId);
        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }
        
        return Ok(user);
    }

    /// <summary>
    /// Update current user
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserDto updateDto, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Updating current user: {UserId}", userId);
        var user = await _userService.UpdateAsync(userId, updateDto, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Delete current user
    /// </summary>
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        _logger.LogInformation("Deleting current user: {UserId}", userId);
        var result = await _userService.DeleteAsync(userId, cancellationToken);
        
        if (!result)
        {
            return NotFound(new { error = "User not found" });
        }
        
        return NoContent();
    }
}