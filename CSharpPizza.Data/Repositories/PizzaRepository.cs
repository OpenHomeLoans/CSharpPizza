using Microsoft.EntityFrameworkCore;
using CSharpPizza.Data.Entities;

namespace CSharpPizza.Data.Repositories;

public class PizzaRepository : Repository<Pizza>, IPizzaRepository
{
    public PizzaRepository(PizzaDbContext context) : base(context)
    {
    }

    public async Task<Pizza?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.PizzaToppings)
            .ThenInclude(pt => pt.Topping)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Pizza>> GetPizzasWithToppingsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.PizzaToppings)
            .ThenInclude(pt => pt.Topping)
            .ToListAsync(cancellationToken);
    }
}