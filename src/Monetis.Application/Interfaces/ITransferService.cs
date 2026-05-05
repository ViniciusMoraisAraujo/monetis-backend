using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ITransferService
{
    Task<TransferResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<TransferResponse>> GetAllAsync();
    Task<TransferResponse> CreateAsync(CreateTransferRequest createDto);
    Task UpdateAsync(Guid id, UpdateTransferRequest updateDto);
    Task DeleteAsync(Guid id);
}
