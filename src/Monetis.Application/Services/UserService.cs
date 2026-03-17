using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService) : IUserService
{
    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdReadOnlyAsync(id);
        return user == null ? null : new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email));
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        var hash = passwordHasher.Hash(createDto.Password);
        var user = new User(createDto.FirstName, createDto.LastName, createDto.Email, hash); 
        await userRepository.Create(user);
        await unitOfWork.CommitAsync();
        return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task UpdateAsync(Guid id, UpdateUserDto updateDto)
    {
        var user = await userRepository.GetByIdReadOnlyAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.Update(updateDto.FirstName, updateDto.LastName, updateDto.Email);
        
        userRepository.Update(user);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await userRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }

    public async Task<string> LoginAsync(LoginUserDto loginDto)
    {
        var user = await userRepository.GetUserByEmailAsync(loginDto.email);
        
        if (user == null)
            throw new Exception();

        var passwordIsValid = passwordHasher.Verify(loginDto.password, user.PasswordHash);
        if(!passwordIsValid)
            throw new Exception();

        var token = tokenService.GenerateToken(user.Id, user.Email);
        return token;
    }
}