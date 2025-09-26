using ProductService.Core.DTOs.ProductDTOs;
using ProductService.Core.Entities;

namespace ProductService.Core.Mappers;

public static class ProductMapper
{
    public static Product MapToProduct(this CreateProductRequestDto request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };
    }

    public static ProductResponseDto MapToProductResponse(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}