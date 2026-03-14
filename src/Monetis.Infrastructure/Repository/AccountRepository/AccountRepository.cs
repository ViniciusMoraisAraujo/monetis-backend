using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository.AccountRepository;

public class AccountRepository(MonetisDataContext context) : IAccountRepository
{
    public async Task<Account?> GetByIdReadOnlyAsync(Guid id)
        => await context.Accounts.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await context.Accounts.AsNoTracking().ToListAsync();
    }

    public async Task CreateAsync(Account entity)
    {
        context.Accounts.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account entity)
    {
        context.Accounts.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
        if (account == null)
            throw new KeyNotFoundException();

        context.Accounts.Remove(account);
        await context.SaveChangesAsync();
    }
}