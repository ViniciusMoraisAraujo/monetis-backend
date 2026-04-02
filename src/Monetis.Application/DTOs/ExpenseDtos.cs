using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record ExpenseDto(
    Guid Id, 
    Guid AccountId, 
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime DueDate, 
    TransactionStatus Status, 
    DateTime? PaidAt);

public record CreateExpenseDto(
    Guid AccountId, 
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime DueDate);

public record UpdateExpenseDto(
    Guid CategoryId, 
    decimal Amount, 
    string Description);