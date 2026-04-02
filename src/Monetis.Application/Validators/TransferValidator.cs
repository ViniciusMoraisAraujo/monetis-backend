using FluentValidation;
using Monetis.Application.DTOs;

namespace Monetis.Application.Validators;

public class CreateTransferDtoValidator : AbstractValidator<CreateTransferDto>
{
    public CreateTransferDtoValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Source account is required");

        RuleFor(x => x.DestinationAccountId)
            .NotEmpty()
            .WithMessage("Destination account is required")
            .NotEqual(x => x.AccountId)
            .WithMessage("Destination account must be different from source account");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Amount cannot exceed 9.999.999.999");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters");

        RuleFor(x => x.TransferredAt)
            .NotEmpty()
            .WithMessage("Transfer date is required")
            .Must(x => x <= DateTime.UtcNow)
            .WithMessage("Transfer date cannot be in the future");
    }
}

public class UpdateTransferDtoValidator : AbstractValidator<UpdateTransferDto>
{
    public UpdateTransferDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Amount cannot exceed 9.999.999.999");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters");
    }
}
