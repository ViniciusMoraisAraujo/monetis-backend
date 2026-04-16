using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services.UserServices;

public class UserAuthServiceService(ITokenService tokenService, IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserAuthService
{
    public async Task<string> LoginAsync(LoginUserDto loginDto)
    {
        var user = await userRepository.GetUserByEmailAsync(loginDto.Email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var passwordIsValid = passwordHasher.Verify(loginDto.Password, user.PasswordHash);
        if (!passwordIsValid)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = tokenService.GenerateToken(user.Id, user.Email);
        return token;
    }
}