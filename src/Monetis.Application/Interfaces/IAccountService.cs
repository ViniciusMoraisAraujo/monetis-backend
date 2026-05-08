using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IAccountService
{
    Task<AccountResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AccountResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AccountResponse> CreateAsync(CreateAccountRequest createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateAccountRequest updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
