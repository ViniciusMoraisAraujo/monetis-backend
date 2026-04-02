using FluentValidation;
using Monetis.Application.DTOs;

namespace Monetis.Application.Validators;

public class CreateIncomeDtoValidator : AbstractValidator<CreateIncomeDto>
{
    public CreateIncomeDtoValidator()
    {
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

        RuleFor(x => x.ReceivedAt)
            .NotEmpty()
            .WithMessage("Received date is required")
            .Must(x => x <= DateTime.UtcNow)
            .WithMessage("Received date cannot be in the future");
    }
}

public class UpdateIncomeDtoValidator : AbstractValidator<UpdateIncomeDto>
{
    public UpdateIncomeDtoValidator()
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

        RuleFor(x => x.ReceivedAt)
            .NotEmpty()
            .WithMessage("Received date is required")
            .Must(x => x <= DateTime.UtcNow)
            .WithMessage("Received date cannot be in the future");
    }
}
