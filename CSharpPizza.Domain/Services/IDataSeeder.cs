namespace CSharpPizza.Domain.Services;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}