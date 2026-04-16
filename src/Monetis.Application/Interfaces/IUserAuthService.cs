using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IUserAuthService
{
    Task<string> LoginAsync(LoginUserDto loginDto);
}