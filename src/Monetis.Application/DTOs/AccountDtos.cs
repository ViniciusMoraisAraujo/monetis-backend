using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record AccountDto(Guid Id, string Name, Guid UserId, AccountType Type, decimal Balance, Currency Currency);
public record CreateAccountDto(string Name, Guid UserId, AccountType Type, Currency Currency);
public record UpdateAccountDto(string Name);