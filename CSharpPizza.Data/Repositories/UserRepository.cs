using Microsoft.EntityFrameworkCore;
using CSharpPizza.Data.Entities;

namespace CSharpPizza.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(PizzaDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}