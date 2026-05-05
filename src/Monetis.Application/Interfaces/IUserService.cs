using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> CreateAsync(CreateUserRequest createDto);
    Task UpdateAsync(Guid id, UpdateUserRequest updateDto);
    Task DeleteAsync(Guid id);
}
