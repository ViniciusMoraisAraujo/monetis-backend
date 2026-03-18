using FluentValidation;
using Monetis.Application.DTOs;

namespace Monetis.Application.Validators;

public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Amount cannot exceed 999,999.99");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(100)
            .WithMessage("Description cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Description can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid frequency type");

        RuleFor(x => x.NextDueDate)
            .NotEmpty()
            .WithMessage("Next due date is required")
            .Must(x => x > DateTime.UtcNow)
            .WithMessage("Next due date must be in the future");
    }
}

public class UpdateSubscriptionDtoValidator : AbstractValidator<UpdateSubscriptionDto>
{
    public UpdateSubscriptionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Amount cannot exceed 999,999.99");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(100)
            .WithMessage("Description cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Description can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid frequency type");

        RuleFor(x => x.NextDueDate)
            .NotEmpty()
            .WithMessage("Next due date is required")
            .Must(x => x > DateTime.UtcNow)
            .WithMessage("Next due date must be in the future");
    }
}
