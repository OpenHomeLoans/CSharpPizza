namespace CSharpPizza.Data.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}