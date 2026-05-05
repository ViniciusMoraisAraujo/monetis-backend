namespace Monetis.Application.DTOs;

public record UserResponse(Guid Id, string FirstName, string LastName, string Email);
public record CreateUserRequest(string FirstName, string LastName, string Email, string Password);
public record UpdateUserRequest(string FirstName, string LastName, string Email);
public record LoginUserRequest(string Email, string Password);
