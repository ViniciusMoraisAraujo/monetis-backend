using Monetis.Application.DTOs;

namespace Monetis.Application.Abstractions.Services;

public interface IUserAuthService
{
    Task<string> LoginAsync(LoginUserRequest loginDto, CancellationToken cancellationToken = default);
}
