using CSharpPizza.Data.Entities;

namespace CSharpPizza.Data.Repositories;

public class LogRepository : Repository<Log>, ILogRepository
{
    public LogRepository(PizzaDbContext context) : base(context)
    {
    }
}