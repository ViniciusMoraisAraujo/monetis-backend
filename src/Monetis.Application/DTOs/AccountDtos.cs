using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record AccountResponse(Guid Id, string Name, Guid UserId, AccountType Type, decimal Balance);
public record CreateAccountRequest(string Name, AccountType Type);
public record UpdateAccountRequest(string Name);
