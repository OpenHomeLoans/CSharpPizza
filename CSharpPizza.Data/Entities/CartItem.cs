namespace CSharpPizza.Data.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid PizzaId { get; set; }
    public Pizza Pizza { get; set; } = null!;

    public int Quantity { get; set; }

    // Navigation properties
    public ICollection<CartItemTopping> CartItemToppings { get; set; } = new List<CartItemTopping>();
}