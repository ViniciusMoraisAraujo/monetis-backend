using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponse> CreateAsync(CreateUserRequest createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateUserRequest updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
