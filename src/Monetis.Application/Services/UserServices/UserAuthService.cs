using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services.UserServices;

public class UserAuthService(ITokenService tokenService, IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserAuthService
{
    public async Task<string> LoginAsync(LoginUserRequest loginDto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByEmailAsync(loginDto.Email, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var passwordIsValid = passwordHasher.Verify(loginDto.Password, user.PasswordHash);
        if (!passwordIsValid)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = tokenService.GenerateToken(user.Id, user.Email);
        return token;
    }
}
