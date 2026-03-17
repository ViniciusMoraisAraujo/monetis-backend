using FluentValidation;
using Monetis.Application.DTOs;

namespace Monetis.Application.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(50)
            .WithMessage("Category name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Category name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid transaction type");

        RuleFor(x => x.Icon)
            .NotEmpty()
            .WithMessage("Icon is required")
            .MaximumLength(20)
            .WithMessage("Icon cannot exceed 20 characters")
            .Matches(@"^[a-zA-Z0-9\-_]+$")
            .WithMessage("Icon can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Name)
            .MustNotBeNullOrWhiteSpace()
            .WithMessage("Category name cannot be empty or whitespace");
    }
}

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(50)
            .WithMessage("Category name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Category name can only contain letters, numbers, spaces, hyphens, and underscores");

        RuleFor(x => x.Icon)
            .NotEmpty()
            .WithMessage("Icon is required")
            .MaximumLength(20)
            .WithMessage("Icon cannot exceed 20 characters")
            .Matches(@"^[a-zA-Z0-9\-_]+$")
            .WithMessage("Icon can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Name)
            .MustNotBeNullOrWhiteSpace()
            .WithMessage("Category name cannot be empty or whitespace");
    }
}
