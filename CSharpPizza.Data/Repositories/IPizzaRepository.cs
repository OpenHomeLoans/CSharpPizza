using CSharpPizza.Data.Entities;

namespace CSharpPizza.Data.Repositories;

public interface IPizzaRepository : IRepository<Pizza>
{
    Task<Pizza?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pizza>> GetPizzasWithToppingsAsync(CancellationToken cancellationToken = default);
}