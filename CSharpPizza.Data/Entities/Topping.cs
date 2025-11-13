namespace CSharpPizza.Data.Entities;

public class Topping : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }

    // Navigation properties
    public ICollection<PizzaTopping> PizzaToppings { get; set; } = new List<PizzaTopping>();
    public ICollection<CartItemTopping> CartItemToppings { get; set; } = new List<CartItemTopping>();
}