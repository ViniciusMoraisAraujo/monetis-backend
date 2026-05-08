using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IUserAuthService
{
    Task<string> LoginAsync(LoginUserRequest loginDto, CancellationToken cancellationToken = default);
}
