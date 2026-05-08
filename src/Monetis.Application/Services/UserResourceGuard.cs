using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Security;
using Monetis.Application.Abstractions.Services;
using Monetis.Domain.Entities;

namespace Monetis.Application.Services;

public class UserResourceGuard(
    IUserContextAccessor userContextAccessor,
    IAccountRepository accountRepository,
    ICardRepository cardRepository,
    ICategoryRepository categoryRepository)
    : IUserResourceGuard
{
    public Guid CurrentUserId
    {
        get
        {
            if (!userContextAccessor.IsResolved)
                throw new UnauthorizedAccessException("User context is not available.");
            
            return userContextAccessor.UserId;
        }
    }

    public async Task<Account> GetOwnedAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {accountId} not found.");

        return account;
    }

    public async Task<Card> GetOwnedCardAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
            throw new KeyNotFoundException($"Card with id {cardId} not found.");

        return card;
    }

    public async Task EnsureOptionalCardBelongsToUserAsync(Guid? cardId, CancellationToken cancellationToken = default)
    {
        if (!cardId.HasValue)
            return;

        _ = await GetOwnedCardAsync(cardId.Value, cancellationToken);
    }

    public async Task<Category> GetVisibleCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        _ = CurrentUserId;

        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {categoryId} not found.");

        return category;
    }

    public async Task<Category> GetOwnedCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await GetVisibleCategoryAsync(categoryId, cancellationToken);
        if (category.UserId != CurrentUserId)
            throw new KeyNotFoundException($"Category with id {categoryId} not found.");

        return category;
    }
}
