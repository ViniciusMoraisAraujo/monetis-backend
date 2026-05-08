using Monetis.Application.DTOs;

namespace Monetis.Application.Abstractions.Services;

public interface ISubscriptionService
{
    Task<SubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SubscriptionResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SubscriptionResponse> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateSubscriptionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
