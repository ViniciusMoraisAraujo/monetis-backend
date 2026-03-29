using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Enums;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class TransactionRepository(MonetisDataContext context)
    : BaseRepository<Transaction>(context), ITransactionRepository
{

    public async Task<IEnumerable<Transaction>> GetByUserAsync(Guid userId)
    {
        return await context.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAllByPeriodAsync(
        DateTime startDate, DateTime endDate, bool descending)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);

        query = descending
            ? query.OrderByDescending(x => x.CreatedAt)
            : query.OrderBy(x => x.CreatedAt);

        return await query.ToListAsync();
    }
}