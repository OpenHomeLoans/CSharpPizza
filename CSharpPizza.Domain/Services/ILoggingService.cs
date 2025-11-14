namespace CSharpPizza.Domain.Services;

public interface ILoggingService
{
    Task LogAsync(string level, string message, string? details = null, int? userId = null, string? endpoint = null, string? httpMethod = null, int? statusCode = null);
    Task LogInfoAsync(string message, string? details = null);
    Task LogErrorAsync(string message, string? details = null);
}