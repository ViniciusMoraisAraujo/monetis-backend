using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto createDto);
    Task UpdateAsync(Guid id, UpdateUserDto updateDto);
    Task DeleteAsync(Guid id);
}