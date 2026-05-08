using Monetis.Domain.Entities;

namespace Monetis.Application.Abstractions.Persistence;
public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<IEnumerable<Subscription>> GetByCategoryReadOnlyAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetByUserReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default);
    
}
