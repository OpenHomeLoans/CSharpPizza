using CSharpPizza.Data.Entities;
using CSharpPizza.Data.Repositories;

namespace CSharpPizza.Domain.Services;

public class LoggingService : ILoggingService
{
    private readonly ILogRepository _logRepository;

    public LoggingService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public Task LogAsync(string level, string message, string? details = null, int? userId = null, string? endpoint = null, string? httpMethod = null, int? statusCode = null)
    {      
        Task.Run(async () =>
        {
            var log = new Log
            {
                Id = Guid.NewGuid(),
                LogLevel = level,
                Message = message,
                Details = details,
                UserId = userId,
                Endpoint = endpoint,
                HttpMethod = httpMethod,
                StatusCode = statusCode
            };

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
        });

        return Task.CompletedTask;
    }

    public Task LogInfoAsync(string message, string? details = null)
    {
        return LogAsync("Info", message, details);
    }

    public Task LogErrorAsync(string message, string? details = null)
    {
        return LogAsync("Error", message, details);
    }
}