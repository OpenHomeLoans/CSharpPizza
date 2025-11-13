namespace CSharpPizza.DTO.Carts;

public class ToppingCustomizationDto
{
    public Guid ToppingId { get; set; }
    public string ToppingName { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public bool IsAdded { get; set; }
}