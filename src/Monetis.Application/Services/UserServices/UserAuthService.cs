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

    public async Task ChangePasswordLoggedInAsync(ChangePasswordLoggedInRequest changePasswordLoggedInDto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userContextAccessor.UserId, cancellationToken)
                   ?? throw new UnauthorizedAccessException();
        var passwordIsValid = passwordHasher.Verify(changePasswordLoggedInDto.CurrentPassword, user.PasswordHash);

        if (!passwordIsValid)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var newPasswordHash = passwordHasher.Hash(changePasswordLoggedInDto.NewPassword);
        user.ChangePassword(newPasswordHash);
        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task ChangePasswordLoggedOutAsync(ChangePasswordLoggedOutRequest changePasswordLoggedOutDto,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByEmailAsync(changePasswordLoggedOutDto.Email, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException();

        var newPasswordHash = passwordHasher.Hash(changePasswordLoggedOutDto.NewPassword);
        user.ChangePassword(newPasswordHash);

        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
