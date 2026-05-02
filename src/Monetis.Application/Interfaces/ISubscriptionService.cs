using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<SubscriptionResponse>> GetAllAsync();
    Task<SubscriptionResponse> CreateAsync(CreateSubscriptionRequest request, Guid userId);
    Task UpdateAsync(Guid id, UpdateSubscriptionRequest request);
    Task DeleteAsync(Guid id);
}
