using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IAccountService
{
    Task<AccountResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<AccountResponse>> GetAllAsync();
    Task<AccountResponse> CreateAsync(CreateAccountRequest createDto);
    Task UpdateAsync(Guid id, UpdateAccountRequest updateDto);
    Task DeleteAsync(Guid id);
}
