using Monetis.Domain.Entities;

namespace Monetis.Application.Abstractions.Services;

public interface IUserResourceGuard
{
    Guid CurrentUserId { get; }

    Task<Account> GetOwnedAccountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<Card> GetOwnedCardAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task EnsureOptionalCardBelongsToUserAsync(
        Guid? cardId,
        CancellationToken cancellationToken = default);

    Task<Category> GetVisibleCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);

    Task<Category> GetOwnedCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
}