using CSharpPizza.Domain.Services;
using CSharpPizza.DTO.Toppings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpPizza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToppingsController : ControllerBase
{
    private readonly IToppingService _toppingService;
    private readonly ILogger<ToppingsController> _logger;

    public ToppingsController(IToppingService toppingService, ILogger<ToppingsController> logger)
    {
        _toppingService = toppingService;
        _logger = logger;
    }

    /// <summary>
    /// Get all toppings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ToppingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ToppingDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all toppings");
        var toppings = await _toppingService.GetAllAsync(cancellationToken);
        return Ok(toppings);
    }

    /// <summary>
    /// Get topping by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToppingDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting topping with ID: {Id}", id);
        var topping = await _toppingService.GetByIdAsync(id, cancellationToken);
        
        if (topping == null)
        {
            return NotFound(new { error = $"Topping with ID {id} not found" });
        }
        
        return Ok(topping);
    }

    /// <summary>
    /// Create a new topping (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ToppingDto>> Create([FromBody] CreateToppingDto createDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new topping: {Name}", createDto.Name);
        var topping = await _toppingService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = topping.Id }, topping);
    }

    /// <summary>
    /// Update a topping (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ToppingDto>> Update(Guid id, [FromBody] UpdateToppingDto updateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating topping with ID: {Id}", id);
        var topping = await _toppingService.UpdateAsync(id, updateDto, cancellationToken);
        
        if (topping == null)
        {
            return NotFound(new { error = $"Topping with ID {id} not found" });
        }
        
        return Ok(topping);
    }

    /// <summary>
    /// Delete a topping (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting topping with ID: {Id}", id);
        var result = await _toppingService.DeleteAsync(id, cancellationToken);
        
        if (!result)
        {
            return NotFound(new { error = $"Topping with ID {id} not found" });
        }
        
        return NoContent();
    }
}