using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;


public record ExpenseResponse(
    Guid Id,
    Guid AccountId,
    Guid CategoryId,
    TransactionStatus Status,
    DateTime? PaidAt,
    decimal Amount,
    string Description,
    DateTime DueDate,
    PaymentMethod PaymentMethod,
    int? InstallmentNumber,
    int? TotalInstallments,
    Guid? InstallmentGroupId,
    Guid? CreditCardId);
public record CreateExpenseRequest(
    Guid UserId,
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime DueDate,
    PaymentMethod PaymentMethod,
    Guid? CreditCardId
);

public record CreateInstallmentRequest(
    Guid UserId,
    Guid AccountId,
    Guid CategoryId,
    decimal TotalAmount,
    string Description,
    DateTime FirstDueDate,
    int NumberOfInstallments,
    PaymentMethod PaymentMethod,
    Guid? CreditCardId
);

public record PayExpenseRequest(
    DateTime PaidAt,
    Guid? AccountId
);

public record UpdateExpenseRequest(
    Guid CategoryId,
    decimal Amount,
    string Description,
    DateTime DueDate,
    bool? IsUpdatingGroup
);

public record UpdateInstallmentGroupRequest(
    Guid NewAccountId,
    Guid CategoryId,
    decimal NewTotalAmount,
    string Description,
    DateTime FirstDueDate,
    PaymentMethod PaymentMethod,
    Guid CreditCardId
);