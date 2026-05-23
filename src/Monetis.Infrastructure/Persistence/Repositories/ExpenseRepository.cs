using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Enums;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class ExpenseRepository(MonetisDataContext context) : BaseRepository<Expense>(context), IExpenseRepository
{
    public async Task<IEnumerable<Expense>> GetByUserReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByStatusReadOnlyAsync(
        TransactionStatus status,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<Expense>()
            .IgnoreQueryFilters()
            .Where(x => x.DueDate < DateTime.UtcNow && x.Status ==  TransactionStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        var query = context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.DueDate >= startDate && x.DueDate <= endDate);
        
        query = descending
            ? query.OrderByDescending(x => x.DueDate)
            : query.OrderBy(x => x.DueDate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }
}
