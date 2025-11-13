namespace CSharpPizza.Data.Entities;

public class CartItemTopping
{
    public Guid CartItemId { get; set; }
    public CartItem CartItem { get; set; } = null!;

    public Guid ToppingId { get; set; }
    public Topping Topping { get; set; } = null!;

    public bool IsAdded { get; set; }
}