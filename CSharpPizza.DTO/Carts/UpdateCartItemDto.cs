using System.ComponentModel.DataAnnotations;

namespace CSharpPizza.DTO.Carts;

public class UpdateCartItemDto
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }

    public List<Guid> AddedToppingIds { get; set; } = new();
    public List<Guid> RemovedToppingIds { get; set; } = new();
}