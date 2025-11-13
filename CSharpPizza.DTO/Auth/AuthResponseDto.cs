namespace CSharpPizza.DTO.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserInfo User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class UserInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}