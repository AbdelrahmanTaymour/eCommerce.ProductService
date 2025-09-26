using FluentValidation;
using ProductService.Core.DTOs.ProductDTOs;

namespace ProductService.Core.Validators.ProductValidators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequestDto>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .Length(1, 100)
            .WithMessage("Product name must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s&\-.,()]+$")
            .WithMessage("Product name contains invalid characters.");

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("Product description cannot exceed 255 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Product price is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Product price must be greater than or equal to 0.")
            .ScalePrecision(2, 10)
            .WithMessage("Product price must not exceed 10 digits with 2 decimal places.");

        RuleFor(x => x.Stock)
            .NotNull()
            .WithMessage("Product stock is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Product stock must be greater than or equal to 0.")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Product stock cannot exceed 1,000,000 units.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required.")
            .GreaterThan(0)
            .WithMessage("Category ID must be a positive integer.");
    }
}