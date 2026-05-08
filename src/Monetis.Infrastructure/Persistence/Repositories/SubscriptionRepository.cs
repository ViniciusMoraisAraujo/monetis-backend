using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class SubscriptionRepository(MonetisDataContext context) : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<IEnumerable<Subscription>> GetByCategoryReadOnlyAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await context.Subscriptions.AsNoTracking().Where(x => x.CategoryId == categoryId).ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Subscription>> GetByUserReadOnlyAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return  await context.Subscriptions.AsNoTracking().Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }
}
