using Microsoft.Extensions.Logging;

using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;
using Monetis.Domain.Entities;

namespace Monetis.Application.Services;

public class CardService(
    ICardRepository cardRepository,
    IUnitOfWork unitOfWork,
    IUserResourceGuard userResourceGuard)
    : ICardService
{
    public async Task<CardResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var card = await cardRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return card == null ? null : new CardResponse(card.Id, card.Name, card.UserId);
    }

    public async Task<IEnumerable<CardResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cards = await cardRepository.GetAllReadOnlyAsync(cancellationToken);
        return cards.Select(c => new CardResponse(c.Id, c.Name, c.UserId));
    }

    public async Task<CardResponse> CreateAsync(CreateCardRequest createDto, CancellationToken cancellationToken = default)
    {
        var card = new Card(createDto.Name);
        cardRepository.Create(card);
        await unitOfWork.CommitAsync(cancellationToken);
        return new CardResponse(card.Id, card.Name, card.UserId);
    }

    public async Task UpdateAsync(Guid id, UpdateCardRequest updateDto, CancellationToken cancellationToken = default)
    {
        var card = await userResourceGuard.GetOwnedCardAsync(id, cancellationToken);

        card.Update(updateDto.Name);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = await userResourceGuard.GetOwnedCardAsync(id, cancellationToken);
        await cardRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
