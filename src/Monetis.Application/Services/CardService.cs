using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class CardService(
    ICardRepository cardRepository,
    IUnitOfWork unitOfWork,
    ILogger<CardService> logger)
    : ICardService
{
    public async Task<CardResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting card by id: {Id}", id);
        var card = await cardRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return card == null ? null : new CardResponse(card.Id, card.Name, card.UserId);
    }

    public async Task<IEnumerable<CardResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all cards");
        var cards = await cardRepository.GetAllReadOnlyAsync(cancellationToken);
        return cards.Select(c => new CardResponse(c.Id, c.Name, c.UserId));
    }

    public async Task<CardResponse> CreateAsync(CreateCardRequest createDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating card: {Name}", createDto.Name);
        var card = new Card(createDto.Name);
        cardRepository.Create(card);
        await unitOfWork.CommitAsync(cancellationToken);
        return new CardResponse(card.Id, card.Name, card.UserId);
    }

    public async Task UpdateAsync(Guid id, UpdateCardRequest updateDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating card: {Id}", id);
        var card = await cardRepository.GetByIdAsync(id, cancellationToken);
        if (card == null)
            throw new KeyNotFoundException($"Card with id {id} not found.");

        card.Update(updateDto.Name);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting card: {Id}", id);
        await cardRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
