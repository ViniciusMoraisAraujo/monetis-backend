using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record TransactionDto(Guid Id, Guid UserId, Guid AccountId, Guid CategoryId, decimal Amount, TransactionType Type, DateTime PaidAt, string Description);
public record CreateTransactionDto(Guid UserId, Guid AccountId, Guid CategoryId, decimal Amount, TransactionType Type, DateTime PaidAt, string Description);
public record UpdateTransactionDto(Guid CategoryId, decimal Amount, string Description);