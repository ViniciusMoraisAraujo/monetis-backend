using Monetis.Domain.Entities;

namespace Monetis.Domain.Interfaces;
public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<IEnumerable<Subscription>> GetByCategoryReadOnlyAsync(Guid categoryId);
    Task<IEnumerable<Subscription>> GetByUserReadOnlyAsync(Guid userId);
    
}