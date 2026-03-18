using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record TransactionDto(Guid Id, Guid AccountId, Guid CategoryId, 
    decimal Amount, TransactionType Type, DateTime? DueDate, DateTime? PaidAt, string Description, TransactionStatus Status);
public record CreateTransactionDto(Guid AccountId, Guid CategoryId, decimal Amount, 
    TransactionType Type, DateTime? DueDate, DateTime? PaidAt, string Description);
public record UpdateTransactionDto(Guid CategoryId, decimal Amount, DateTime? DueDate, 
    DateTime? PaidAt, TransactionStatus Status, string Description);