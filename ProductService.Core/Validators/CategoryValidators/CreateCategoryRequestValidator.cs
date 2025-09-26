using FluentValidation;
using ProductService.Core.DTOs.CategoryDTOs;

namespace ProductService.Core.Validators.CategoryValidators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDto>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .Length(1, 100).WithMessage("Category name must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s&-]+$")
            .WithMessage("Category name can only contain letters, numbers, spaces, ampersands, and hyphens.");
    }
}