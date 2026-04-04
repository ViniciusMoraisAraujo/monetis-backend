using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record AccountDto(Guid Id, string Name, Guid UserId, AccountType Type, decimal Balance);
public record CreateAccountDto(string Name,  AccountType Type);
public record UpdateAccountDto(string Name);