using FluentValidation;
using Monetis.Application.DTOs;

namespace Monetis.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(50)
            .WithMessage("Category name cannot exceed 50 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Category name can only contain letters and spaces");
        
        RuleFor(x => x.Icon)
            .NotEmpty()
            .WithMessage("Icon is required")
            .MaximumLength(15) 
            .Matches(@"^[\p{L}\p{N}\p{P}\p{S}\p{Cs}]+$") 
            .WithMessage("Icon must be a valid character or emoji");
        
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(50)
            .WithMessage("Category name cannot exceed 50 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Category name can only contain letters and spaces");
        
        RuleFor(x => x.Icon)
            .NotEmpty()
            .WithMessage("Icon is required")
            .MaximumLength(15) 
            .Matches(@"^[\p{L}\p{N}\p{P}\p{S}\p{Cs}]+$") 
            .WithMessage("Icon must be a valid character or emoji");
    }
}
