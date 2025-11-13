namespace CSharpPizza.DTO.Pizzas;

public class PizzaListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal ComputedCost { get; set; }
    public string? ImageUrl { get; set; }
    public string Slug { get; set; } = string.Empty;
}