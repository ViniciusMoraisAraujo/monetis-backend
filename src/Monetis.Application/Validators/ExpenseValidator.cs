using FluentValidation;
using Monetis.Application.DTOs;
using Monetis.Domain.Enums;

namespace Monetis.Application.Validators;

public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User is required");

        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Amount cannot exceed 9.999.999.999");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters")
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required")
            .Must(x => x >= DateTime.UtcNow.Date.AddYears(-1))
            .WithMessage("Due date is too far in the past");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("Invalid payment method");

        RuleFor(x => x.CreditCardId)
            .NotEmpty()
            .WithMessage("Credit card is required for credit card payments")
            .When(x => x.PaymentMethod == PaymentMethod.CreditCard);

        RuleFor(x => x.CreditCardId)
            .Must(x => !x.HasValue)
            .WithMessage("Credit card should only be set for credit card payments")
            .When(x => x.PaymentMethod != PaymentMethod.CreditCard);
    }
}

public class CreateInstallmentRequestValidator : AbstractValidator<CreateInstallmentRequest>
{
    public CreateInstallmentRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User is required");

        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Total amount cannot exceed 9.999.999.999");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters")
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.FirstDueDate)
            .NotEmpty()
            .WithMessage("First due date is required");

        RuleFor(x => x.NumberOfInstallments)
            .InclusiveBetween(2, 24)
            .WithMessage("Installments must be between 2 and 24");

        RuleFor(x => x.PaymentMethod)
            .Equal(PaymentMethod.CreditCard)
            .WithMessage("Installments must use credit card payment method");

        RuleFor(x => x.CreditCardId)
            .NotEmpty()
            .WithMessage("Credit card is required for installments");
    }
}

public class PayExpenseRequestValidator : AbstractValidator<PayExpenseRequest>
{
    public PayExpenseRequestValidator()
    {
        RuleFor(x => x.PaidAt)
            .NotEmpty()
            .WithMessage("Payment date is required")
            .Must(x => x <= DateTime.UtcNow)
            .WithMessage("Payment date cannot be in the future");
    }
}

public class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
{
    public UpdateExpenseRequestValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Amount cannot exceed 9.999.999.999");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters")
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required")
            .Must(x => x >= DateTime.UtcNow.Date.AddYears(-1))
            .WithMessage("Due date is too far in the past");
    }
}

public class UpdateInstallmentGroupRequestValidator : AbstractValidator<UpdateInstallmentGroupRequest>
{
    public UpdateInstallmentGroupRequestValidator()
    {
        RuleFor(x => x.NewAccountId)
            .NotEmpty()
            .WithMessage("Account is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required");

        RuleFor(x => x.NewTotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Total amount cannot exceed 9.999.999.999");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters")
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.FirstDueDate)
            .NotEmpty()
            .WithMessage("First due date is required");

        RuleFor(x => x.PaymentMethod)
            .Equal(PaymentMethod.CreditCard)
            .WithMessage("Installment groups must use credit card payment method");

        RuleFor(x => x.CreditCardId)
            .NotEmpty()
            .WithMessage("Credit card is required");
    }
}
