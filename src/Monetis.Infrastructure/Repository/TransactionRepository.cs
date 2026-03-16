using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Enums;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data; 

namespace Monetis.Infrastructure.Repository;

public class TransactionRepository(MonetisDataContext context) : BaseRepository<Transaction>(context),ITransactionRepository
{
    public async Task<IEnumerable<Transaction>> GetByCategoryAsync(Guid categoryId)
    {
        return await context.Transactions.AsNoTracking().Where(x => x.CategoryId == categoryId).ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type)
    {
        return await context.Transactions.AsNoTracking().Where(x => x.Type == type).ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByUserAsync(Guid userId)
    {
        return  await context.Transactions.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAllByPeriodAsync(DateTime startDate, DateTime endDate, bool descending)
    {
        var query =  context.Transactions.AsNoTracking()
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);

        query = descending
            ? query.OrderByDescending(x => x.CreatedAt)
            : query.OrderBy(x => x.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAllByStatusAsync(TransactionStatus status)
    {
        return await context.Transactions.AsNoTracking().Where(x => x.Status == status).ToListAsync();
    }
}