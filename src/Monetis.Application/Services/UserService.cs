using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdReadOnlyAsync(id);
        return user == null ? null : new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email));
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        var user = new User(createDto.FirstName, createDto.LastName, createDto.Email, createDto.Password); // Password should be hashed here
        await _userRepository.Create(user);
        await _unitOfWork.CommitAsync();
        return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task UpdateAsync(Guid id, UpdateUserDto updateDto)
    {
        var user = await _userRepository.GetByIdReadOnlyAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.Update(updateDto.FirstName, updateDto.LastName, updateDto.Email);
        
        _userRepository.Update(user);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}