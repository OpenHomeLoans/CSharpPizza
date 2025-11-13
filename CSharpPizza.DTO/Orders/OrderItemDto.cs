namespace CSharpPizza.DTO.Orders;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public string PizzaName { get; set; } = string.Empty;
    public decimal PizzaPrice { get; set; }
    public int Quantity { get; set; }
    public string Toppings { get; set; } = string.Empty;
    public decimal ItemTotal { get; set; }
}