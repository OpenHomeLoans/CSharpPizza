using System.ComponentModel.DataAnnotations;

namespace CSharpPizza.DTO.Pizzas;

public class CreatePizzaDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Base price is required")]
    [Range(0.01, 10000, ErrorMessage = "Base price must be between 0.01 and 10000")]
    public decimal BasePrice { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? ImageUrl { get; set; }

    public List<Guid> ToppingIds { get; set; } = new();
}