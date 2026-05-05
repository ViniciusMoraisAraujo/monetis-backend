using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    IExpenseRepository expenseRepository,
    ILogger<SubscriptionService> logger)
    : ISubscriptionService
{
    public async Task<SubscriptionResponse?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting subscription by id: {Id}", id);

        var subscription = await subscriptionRepository.GetByIdReadOnlyAsync(id);
        return subscription == null ? null : MapToDto(subscription);
    }

    public async Task<IEnumerable<SubscriptionResponse>> GetAllAsync()
    {
        logger.LogInformation("Getting all subscriptions");

        var subscriptions = await subscriptionRepository.GetAllReadOnlyAsync();
        return subscriptions.Select(MapToDto);
    }

    public async Task<SubscriptionResponse> CreateAsync(CreateSubscriptionRequest request)
    {
        logger.LogInformation("Creating subscription");
        var subscription = new Subscription(
            accountId: request.AccountId,
            categoryId: request.CategoryId,
            amount: request.Amount,
            description: request.Description,
            frequency: request.Frequency,
            nextDueDate: request.NextDueDate,
            paymentMethod: request.PaymentMethod);

        var firstExpense = subscription.Process();
        subscriptionRepository.Create(subscription);
        expenseRepository.Create(firstExpense);
        await unitOfWork.CommitAsync();

        return MapToDto(subscription);
    }

    public async Task UpdateAsync(Guid id, UpdateSubscriptionRequest request)
    {
        logger.LogInformation("Updating subscription: {Id}", id);

        var subscription = await subscriptionRepository.GetByIdAsync(id);
        if (subscription == null)
            throw new KeyNotFoundException($"Subscription with id {id} not found.");

        subscription.Update(
            amount: request.Amount,
            description: request.Description,
            frequency: request.Frequency,
            nextDueDate: request.NextDueDate,
            isActive: request.IsActive,
            paymentMethod: subscription.PaymentMethod,
            accountId: subscription.AccountId,
            endDate: subscription.EndDate,
            creditCardId: subscription.CardId);

        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Cancelling subscription: {Id}", id);

        var subscription = await subscriptionRepository.GetByIdAsync(id);
        if (subscription == null)
            throw new KeyNotFoundException($"Subscription with id {id} not found.");

        subscription.Cancel();
        await unitOfWork.CommitAsync();
    }

    private static SubscriptionResponse MapToDto(Subscription subscription)
    {
        return new SubscriptionResponse(
            subscription.Id,
            subscription.Amount,
            subscription.Description,
            subscription.Frequency,
            subscription.NextDueDate,
            subscription.IsActive);
    }
}
