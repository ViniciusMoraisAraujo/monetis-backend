using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto> CreateAsync(CreateAccountDto createDto, Guid userId);
    Task UpdateAsync(Guid id, UpdateAccountDto updateDto);
    Task DeleteAsync(Guid id);
}