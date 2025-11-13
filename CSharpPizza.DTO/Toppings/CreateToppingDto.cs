using System.ComponentModel.DataAnnotations;

namespace CSharpPizza.DTO.Toppings;

public class CreateToppingDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cost is required")]
    [Range(0.01, 1000, ErrorMessage = "Cost must be between 0.01 and 1000")]
    public decimal Cost { get; set; }
}