using Monetis.Domain.Entities;

namespace Monetis.Domain.Interfaces;
public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<IEnumerable<Subscription>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Subscription>> GetByUserAsync(Guid userId);
    
}