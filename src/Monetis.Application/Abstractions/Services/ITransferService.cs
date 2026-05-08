using Monetis.Application.DTOs;

namespace Monetis.Application.Abstractions.Services;

public interface ITransferService
{
    Task<TransferResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransferResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TransferResponse> CreateAsync(CreateTransferRequest createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateTransferRequest updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
