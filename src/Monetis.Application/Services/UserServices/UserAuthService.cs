using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Security;
using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;

namespace Monetis.Application.Services.UserServices;

public class UserAuthService(ITokenService tokenService, IUserRepository userRepository, 
    IPasswordHasher passwordHasher, IUserContextAccessor userContextAccessor,
    IUnitOfWork unitOfWork) : IUserAuthService
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

    public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordDto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByEmailAsync(userContextAccessor.UserId.ToString(), cancellationToken) 
                   ?? throw new UnauthorizedAccessException();
        var passwordIsValid = passwordHasher.Verify(changePasswordDto.CurrentPassword, user.PasswordHash);
        
        if (!passwordIsValid)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var newPasswordHash = passwordHasher.Hash(changePasswordDto.NewPassword);
        user.ChangePassword(newPasswordHash);
        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
