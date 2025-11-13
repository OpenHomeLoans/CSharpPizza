namespace CSharpPizza.DTO.Carts;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid PizzaId { get; set; }
    public string PizzaName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal BasePrice { get; set; }
    public List<ToppingCustomizationDto> CustomToppings { get; set; } = new();
    public decimal ItemTotal { get; set; }
}