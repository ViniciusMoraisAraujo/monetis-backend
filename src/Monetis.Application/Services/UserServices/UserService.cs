using Microsoft.Extensions.Logging;

using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Security;
using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;
using Monetis.Domain.Entities;
using Monetis.Domain.Exceptions;

namespace Monetis.Application.Services.UserServices;

public class UserService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IUserService
{
    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return user == null ? null : new UserResponse(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllReadOnlyAsync(cancellationToken);
        return users.Select(u => new UserResponse(u.Id, u.FirstName, u.LastName, u.Email));
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest createDto, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetUserByEmailAsync(createDto.Email, cancellationToken);

        if (existingUser != null)
            throw new UserAlreadyExistsException(createDto.Email);

        var hash = passwordHasher.Hash(createDto.Password);
        var user = new User(createDto.FirstName, createDto.LastName, createDto.Email.ToLowerInvariant(), hash);
        userRepository.Create(user);
        await unitOfWork.CommitAsync(cancellationToken);
        return new UserResponse(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task UpdateAsync(Guid id, UpdateUserRequest updateDto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.Update(updateDto.FirstName, updateDto.LastName, updateDto.Email);

        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await userRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
