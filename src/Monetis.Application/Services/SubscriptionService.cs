using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetByIdReadOnlyAsync(id);
        return subscription == null ? null : new SubscriptionDto(subscription.Id, subscription.UserId, subscription.AccountId, subscription.CategoryId, subscription.Amount, subscription.Description, subscription.Frequency, subscription.NextDueDate, subscription.IsActive);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return subscriptions.Select(s => new SubscriptionDto(s.Id, s.UserId, s.AccountId, s.CategoryId, s.Amount, s.Description, s.Frequency, s.NextDueDate, s.IsActive));
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto createDto)
    {
        var subscription = new Subscription(createDto.UserId, createDto.AccountId, createDto.CategoryId, createDto.Amount, createDto.Description, createDto.Frequency, createDto.NextDueDate, true);
        await _subscriptionRepository.Create(subscription);
        await _unitOfWork.CommitAsync();
        return new SubscriptionDto(subscription.Id, subscription.UserId, subscription.AccountId, subscription.CategoryId, subscription.Amount, subscription.Description, subscription.Frequency, subscription.NextDueDate, subscription.IsActive);
    }

    public async Task UpdateAsync(Guid id, UpdateSubscriptionDto updateDto)
    {
        var subscription = await _subscriptionRepository.GetByIdReadOnlyAsync(id);
        if (subscription == null)
            throw new KeyNotFoundException($"Subscription with id {id} not found.");

        subscription.Update(updateDto.Amount, updateDto.Description, updateDto.Frequency, updateDto.NextDueDate, updateDto.IsActive);
        
        _subscriptionRepository.Update(subscription);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _subscriptionRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}