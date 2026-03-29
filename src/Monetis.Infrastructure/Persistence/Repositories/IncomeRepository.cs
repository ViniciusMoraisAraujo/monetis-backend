using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class IncomeRepository(MonetisDataContext context) : IIncomeRepository
{
    public async Task<Income?> GetByIdAsync(Guid id)
    {
        return await context.Set<Income>().FindAsync(id);
    }

    public async Task<Income?> GetByIdReadOnlyAsync(Guid id)
    {
        return await context.Set<Income>()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IReadOnlyList<Income>> GetAllReadOnlyAsync()
    {
        return await context.Set<Income>()
            .AsNoTracking()
            .ToListAsync();
    }

    public void Create(Income entity)
    {
        context.Set<Income>().Add(entity);
    }

    public void Update(Income entity)
    {
        context.Set<Income>().Update(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var income = await GetByIdAsync(id);
        if (income != null)
            context.Set<Income>().Remove(income);
    }

    public async Task<IEnumerable<Income>> GetByCategoryAsync(Guid categoryId)
    {
        return await context.Set<Income>()
            .Where(i => i.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Income>> GetByUserAsync(Guid userId)
    {
        return await context.Set<Income>()
            .Where(i => i.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Income>> GetByPeriodAsync(DateTime startDate, DateTime endDate, bool descending)
    {
        var query = context.Set<Income>()
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate);

        query = descending 
            ? query.OrderByDescending(i => i.CreatedAt) 
            : query.OrderBy(i => i.CreatedAt);

        return await query.ToListAsync();
    }
}
