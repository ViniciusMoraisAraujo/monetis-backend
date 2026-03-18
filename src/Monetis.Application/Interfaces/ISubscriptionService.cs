using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<SubscriptionDto>> GetAllAsync();
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto createDto, Guid userId);
    Task UpdateAsync(Guid id, UpdateSubscriptionDto updateDto);
    Task DeleteAsync(Guid id);
}