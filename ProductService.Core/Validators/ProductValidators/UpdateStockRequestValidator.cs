using FluentValidation;
using ProductService.Core.DTOs.ProductDTOs;

namespace ProductService.Core.Validators.ProductValidators;

public class UpdateStockRequestValidator : AbstractValidator<UpdateStockRequestDto>
{
    public UpdateStockRequestValidator()
    {
        RuleFor(x => x.NewStock)
            .NotNull()
            .WithMessage("Stock is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock must be greater than or equal to 0.")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Stock cannot exceed 1,000,000 units.");
    }
}