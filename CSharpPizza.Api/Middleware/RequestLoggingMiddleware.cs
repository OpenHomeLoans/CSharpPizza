using System.Security.Claims;
using CSharpPizza.Domain.Services;

namespace CSharpPizza.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
    {
        
        var method = context.Request.Method;
        var path = context.Request.Path;
        var timestamp = DateTime.UtcNow;
        
        // Get user information if authenticated
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        _logger.LogInformation("Incoming request: {Method} {Path} at {Timestamp} by User: {UserId}",
            method, path, timestamp, userId?.ToString() ?? "Anonymous");

        // Fire-and-forget logging (don't await)
        _ = loggingService.LogAsync(
            level: "Info",
            message: $"API Request: {method} {path}",
            details: $"Timestamp: {timestamp}, User: {userId?.ToString() ?? "Anonymous"}",
            userId: userId,
            endpoint: path,
            httpMethod: method,
            statusCode: null);

        // Call the next middleware in the pipeline
        await _next(context);

        // Log response status code
        var statusCode = context.Response.StatusCode;
        
        _logger.LogInformation("Response: {Method} {Path} returned {StatusCode}",
            method, path, statusCode);

        // Fire-and-forget logging (don't await)
        _ = loggingService.LogAsync(
            level: statusCode >= 400 ? "Error" : "Info",
            message: $"API Response: {method} {path}",
            details: $"StatusCode: {statusCode}, User: {userId?.ToString() ?? "Anonymous"}",
            userId: userId,
            endpoint: path,
            httpMethod: method,
            statusCode: statusCode);
    }
}
