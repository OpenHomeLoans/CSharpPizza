namespace CSharpPizza.Data.Entities;

public class Pizza : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public string Slug { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<PizzaTopping> PizzaToppings { get; set; } = new List<PizzaTopping>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}