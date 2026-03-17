using FluentValidation;
using FluentValidation.Validators;
using Monetis.Application.DTOs;
using Monetis.Domain.Entities;

namespace Monetis.Application.Validators;

public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
{
    public CreateAccountDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name is required")
            .MaximumLength(100)
            .WithMessage("Account name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Account name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid account type");

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Invalid currency type");

        RuleFor(x => x.Name)
            .MustNotBeNullOrWhiteSpace()
            .WithMessage("Account name cannot be empty or whitespace");
    }
}

public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name is required")
            .MaximumLength(100)
            .WithMessage("Account name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Account name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Name)
            .MustNotBeNullOrWhiteSpace()
            .WithMessage("Account name cannot be empty or whitespace");
    }
}
