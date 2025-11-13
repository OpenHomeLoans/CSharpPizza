namespace CSharpPizza.Data.Entities;

public class PizzaTopping
{
    public Guid PizzaId { get; set; }
    public Pizza Pizza { get; set; } = null!;

    public Guid ToppingId { get; set; }
    public Topping Topping { get; set; } = null!;
}