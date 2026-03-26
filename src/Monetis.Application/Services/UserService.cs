using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher, 
    ITokenService tokenService, 
    ILogger<UserService> logger) : IUserService
{
    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting user by id: {Id}", id);
        var user = await userRepository.GetByIdReadOnlyAsync(id);
        return user == null ? null : new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        logger.LogInformation("Getting all users");
        var users = await userRepository.GetAllReadOnlyAsync();
        return users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email));
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        logger.LogInformation("Creating user: {Email}", createDto.Email);
        var hash = passwordHasher.Hash(createDto.Password);
        var user = new User(createDto.FirstName, createDto.LastName, createDto.Email, hash); 
        userRepository.Create(user);
        await unitOfWork.CommitAsync();
        return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task UpdateAsync(Guid id, UpdateUserDto updateDto)
    {
        logger.LogInformation("Updating user: {Id}", id);
        var user = await userRepository.GetByIdReadOnlyAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.Update(updateDto.FirstName, updateDto.LastName, updateDto.Email);
        
        userRepository.Update(user);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting user: {Id}", id);
        await userRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }

    public async Task<string> LoginAsync(LoginUserDto loginDto)
    {
        logger.LogInformation("User login attempt: {Email}", loginDto.email);
        var user = await userRepository.GetUserByEmailAsync(loginDto.email);
        
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var passwordIsValid = passwordHasher.Verify(loginDto.password, user.PasswordHash);
        if(!passwordIsValid)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = tokenService.GenerateToken(user.Id, user.Email);
        logger.LogInformation("User login successful: {Email}", loginDto.email);
        return token;
    }
}
