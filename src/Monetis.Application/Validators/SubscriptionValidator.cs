using FluentValidation;
using Monetis.Application.DTOs;

namespace Monetis.Application.Validators;

public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionDtoValidator()
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
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(100)
            .WithMessage("Description cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s0-9\-_]+$")
            .WithMessage("Description can only contain letters and spaces");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid frequency");
        
        RuleFor(x => x.NextDueDate)
            .NotEmpty()
            .WithMessage("Next due date is required")
            .Must(x => x >= DateTime.UtcNow)
            .WithMessage("Next due date cannot be in the past");
    }
}

public class UpdateSubscriptionDtoValidator : AbstractValidator<UpdateSubscriptionDto>
{
    public UpdateSubscriptionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(9999999999)
            .WithMessage("Amount cannot exceed 9.999.999.999");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(100)
            .WithMessage("Description cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s0-9\-_]+$")
            .WithMessage("Description can only contain letters and spaces");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid frequency");
        
        RuleFor(x => x.NextDueDate)
            .NotEmpty()
            .WithMessage("Next due date is required")
            .Must(x => x >= DateTime.UtcNow)
            .WithMessage("Next due date cannot be in the past");
    }
}
