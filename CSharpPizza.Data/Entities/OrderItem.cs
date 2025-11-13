namespace CSharpPizza.Data.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string PizzaName { get; set; } = string.Empty;
    public decimal PizzaPrice { get; set; }
    public int Quantity { get; set; }
    public string Toppings { get; set; } = string.Empty; // JSON serialized list of toppings
}