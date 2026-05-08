using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities.Transactions;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class IncomeRepository(MonetisDataContext context) : BaseRepository<Income>(context), IIncomeRepository
{
    public async Task<IEnumerable<Income>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Income>()
            .Where(i => i.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Income>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Income>()
            .Where(i => i.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Income>> GetByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        var query = context.Set<Income>()
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate);

        query = descending 
            ? query.OrderByDescending(i => i.CreatedAt) 
            : query.OrderBy(i => i.CreatedAt);

        return await query.ToListAsync(cancellationToken);
    }
}
