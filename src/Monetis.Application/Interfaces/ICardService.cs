using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ICardService
{
    Task<CardResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CardResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CardResponse> CreateAsync(CreateCardRequest createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateCardRequest updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
