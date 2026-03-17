namespace Monetis.Application.DTOs;

public record UserDto(Guid Id, string FirstName, string LastName, string Email);
public record CreateUserDto(string FirstName, string LastName, string Email, string Password);
public record UpdateUserDto(string FirstName, string LastName, string Email);