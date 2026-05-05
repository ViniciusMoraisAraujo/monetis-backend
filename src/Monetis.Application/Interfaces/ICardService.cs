using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ICardService
{
    Task<CardResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<CardResponse>> GetAllAsync();
    Task<CardResponse> CreateAsync(CreateCardRequest createDto);
    Task UpdateAsync(Guid id, UpdateCardRequest updateDto);
    Task DeleteAsync(Guid id);
}
