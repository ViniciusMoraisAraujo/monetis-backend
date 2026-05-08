using Microsoft.Extensions.Logging;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;
using Monetis.Domain.Entities;

namespace Monetis.Application.Services;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    IExpenseRepository expenseRepository,
    IUserResourceGuard userResourceGuard,
    ILogger<SubscriptionService> logger)
    : ISubscriptionService
{
    public async Task<SubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting subscription by id: {Id}", id);

        var subscription = await subscriptionRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return subscription == null ? null : MapToDto(subscription);
    }

    public async Task<IEnumerable<SubscriptionResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all subscriptions");

        var subscriptions = await subscriptionRepository.GetAllReadOnlyAsync(cancellationToken);
        return subscriptions.Select(MapToDto);
    }

    public async Task<SubscriptionResponse> CreateAsync(
        CreateSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating subscription");
        _ = await userResourceGuard.GetOwnedAccountAsync(request.AccountId, cancellationToken);
        _ = await userResourceGuard.GetVisibleCategoryAsync(request.CategoryId, cancellationToken);

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
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToDto(subscription);
    }

    public async Task UpdateAsync(Guid id, UpdateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating subscription: {Id}", id);

        var subscription = await subscriptionRepository.GetByIdAsync(id, cancellationToken);
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

        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Cancelling subscription: {Id}", id);

        var subscription = await subscriptionRepository.GetByIdAsync(id, cancellationToken);
        if (subscription == null)
            throw new KeyNotFoundException($"Subscription with id {id} not found.");

        subscription.Cancel();
        await unitOfWork.CommitAsync(cancellationToken);
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
