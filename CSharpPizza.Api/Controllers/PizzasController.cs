using CSharpPizza.Domain.Services;
using CSharpPizza.DTO.Pizzas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpPizza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly IPizzaService _pizzaService;
    private readonly ILogger<PizzasController> _logger;

    public PizzasController(IPizzaService pizzaService, ILogger<PizzasController> logger)
    {
        _pizzaService = pizzaService;
        _logger = logger;
    }

    /// <summary>
    /// Get all pizzas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PizzaListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PizzaListDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all pizzas");
        var pizzas = await _pizzaService.GetAllAsync(cancellationToken);
        return Ok(pizzas);
    }

    /// <summary>
    /// Get pizza by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PizzaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PizzaDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting pizza with ID: {Id}", id);
        var pizza = await _pizzaService.GetByIdAsync(id, cancellationToken);
        
        if (pizza == null)
        {
            return NotFound(new { error = $"Pizza with ID {id} not found" });
        }
        
        return Ok(pizza);
    }

    /// <summary>
    /// Get pizza by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(PizzaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PizzaDto>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting pizza with slug: {Slug}", slug);
        var pizza = await _pizzaService.GetBySlugAsync(slug, cancellationToken);
        
        if (pizza == null)
        {
            return NotFound(new { error = $"Pizza with slug '{slug}' not found" });
        }
        
        return Ok(pizza);
    }

    /// <summary>
    /// Create a new pizza (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PizzaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PizzaDto>> Create([FromBody] CreatePizzaDto createDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new pizza: {Name}", createDto.Name);
        var pizza = await _pizzaService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = pizza.Id }, pizza);
    }

    /// <summary>
    /// Update a pizza (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PizzaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PizzaDto>> Update(Guid id, [FromBody] UpdatePizzaDto updateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating pizza with ID: {Id}", id);
        var pizza = await _pizzaService.UpdateAsync(id, updateDto, cancellationToken);
        
        if (pizza == null)
        {
            return NotFound(new { error = $"Pizza with ID {id} not found" });
        }
        
        return Ok(pizza);
    }

    /// <summary>
    /// Delete a pizza (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting pizza with ID: {Id}", id);
        var result = await _pizzaService.DeleteAsync(id, cancellationToken);
        
        if (!result)
        {
            return NotFound(new { error = $"Pizza with ID {id} not found" });
        }
        
        return NoContent();
    }
}