using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository.SubscriptionRepository;

public class SubscriptionRepository(MonetisDataContext context) : ISubscriptionRepository
{
    public async Task<Subscription?> GetByIdReadOnlyAsync(Guid id)
    {
        return await context.Subscriptions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await context.Subscriptions.AsNoTracking().ToListAsync();
    }

    public async Task CreateAsync(Subscription entity)
    {
        context.Subscriptions.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Subscription entity)
    {
        context.Subscriptions.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var subscription = await context.Subscriptions.AsNoTracking().FirstOrDefaultAsync();
        if (subscription == null)
            throw new KeyNotFoundException();

        context.Subscriptions.Remove(subscription);
        await context.SaveChangesAsync();
    }
}