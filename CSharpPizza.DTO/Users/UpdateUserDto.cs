using System.ComponentModel.DataAnnotations;

namespace CSharpPizza.DTO.Users;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;
}