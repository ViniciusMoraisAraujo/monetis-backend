using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Enums;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class ExpenseRepository(MonetisDataContext context) : BaseRepository<Expense>(context), IExpenseRepository
{
    public async Task<IEnumerable<Expense>> GetByUserReadOnlyAsync(Guid userId)
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetByStatusReadOnlyAsync(TransactionStatus status)
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetOverdueAsync()
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.DueDate < DateTime.Now && x.Status ==  TransactionStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetByPeriodAsync(DateTime startDate, DateTime endDate, bool descending)
    {
        var query = context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.DueDate >= startDate && x.DueDate <= endDate);
        
        query = descending
            ? query.OrderByDescending(x => x.DueDate)
            : query.OrderBy(x => x.DueDate);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetByCategoryAsync(Guid categoryId)
    {
        return await context.Set<Expense>()
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync();
    }
}