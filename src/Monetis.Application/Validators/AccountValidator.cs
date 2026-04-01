using FluentValidation;
using Monetis.Application.DTOs;

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
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Account name can only contain letters and spaces");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid account type");

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Invalid currency type");
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
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Account name can only contain letters and spaces");
    }
}