using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository.TransactionRepository;

public class TransactionRepository(MonetisDataContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdReadOnlyAsync(Guid id)
        => await context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Transaction>> GetAllAsync()
        => await context.Transactions.AsNoTracking().ToListAsync();

    public async Task CreateAsync(Transaction entity)
    {
        context.Transactions.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction entity)
    {
        context.Transactions.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var transaction = await context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
        if (transaction == null)
            throw new KeyNotFoundException();

        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
    }
}