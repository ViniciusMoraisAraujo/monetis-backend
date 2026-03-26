using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    ILogger<SubscriptionService> logger)
    : ISubscriptionService
{
    public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting subscription by id: {Id}", id);
        var subscription = await subscriptionRepository.GetByIdReadOnlyAsync(id);
        return subscription == null ? null : new SubscriptionDto(subscription.Id, subscription.Amount, subscription.Description, subscription.Frequency, subscription.NextDueDate, subscription.IsActive);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
    {
        logger.LogInformation("Getting all subscriptions");
        var subscriptions = await subscriptionRepository.GetAllReadOnlyAsync();
        return subscriptions.Select(s => new SubscriptionDto(s.Id, s.Amount, s.Description, s.Frequency, s.NextDueDate, s.IsActive));
    }
    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto createDto, Guid userId)
    {
        logger.LogInformation("Creating subscription: {Description}", createDto.Description);
        var subscription = new Subscription(userId, createDto.AccountId, createDto.CategoryId, createDto.Amount, createDto.Description, createDto.Frequency, createDto.NextDueDate, true);
        subscriptionRepository.Create(subscription);
        await unitOfWork.CommitAsync();
        return new SubscriptionDto(subscription.Id, subscription.Amount, subscription.Description, subscription.Frequency, subscription.NextDueDate, subscription.IsActive);
    }

    public async Task UpdateAsync(Guid id, UpdateSubscriptionDto updateDto)
    {
        logger.LogInformation("Updating subscription: {Id}", id);
        var subscription = await subscriptionRepository.GetByIdReadOnlyAsync(id);
        if (subscription == null)
            throw new KeyNotFoundException($"Subscription with id {id} not found.");

        subscription.Update(updateDto.Amount, updateDto.Description, updateDto.Frequency, updateDto.NextDueDate, updateDto.IsActive);
        
        subscriptionRepository.Update(subscription);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting subscription: {Id}", id);
        await subscriptionRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }
}
