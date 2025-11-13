namespace CSharpPizza.Data.Entities;

public class Log : BaseEntity
{
    public string LogLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int? UserId { get; set; }
    public string? Endpoint { get; set; }
    public string? HttpMethod { get; set; }
    public int? StatusCode { get; set; }
}