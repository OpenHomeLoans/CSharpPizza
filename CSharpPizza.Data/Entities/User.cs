namespace CSharpPizza.Data.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole UserRole { get; set; } = UserRole.Customer;

    // Navigation properties
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}