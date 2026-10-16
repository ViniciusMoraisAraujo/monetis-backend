namespace Monetis.Application.DTOs;

public record LoginUserRequest(string Email, string Password);
public record RegisterUserRequest(string FirstName, string LastName, string Email, string PasswordHash);
