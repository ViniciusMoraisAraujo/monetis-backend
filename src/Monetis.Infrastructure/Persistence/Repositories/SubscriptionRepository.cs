using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class SubscriptionRepository(MonetisDataContext context) : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<IEnumerable<Subscription>> GetByCategoryAsync(Guid categoryId)
    {
        return await context.Subscriptions.AsNoTracking().Where(x => x.CategoryId == categoryId).ToListAsync();
    }
    
    public async Task<IEnumerable<Subscription>> GetByUserAsync(Guid userId)
    {
        return  await context.Subscriptions.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
    }
}