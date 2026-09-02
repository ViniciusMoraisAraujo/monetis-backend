using Microsoft.EntityFrameworkCore;

using Monetis.Application.Abstractions.Persistence;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class TransactionRepository(MonetisDataContext context)
    : BaseRepository<Transaction>(context), ITransactionRepository
{
    private readonly MonetisDataContext _context = context;

    public async Task<IEnumerable<Transaction>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetAllByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);

        query = descending
            ? query.OrderByDescending(x => x.CreatedAt)
            : query.OrderBy(x => x.CreatedAt);

        return await query.ToListAsync(cancellationToken);
    }
}
