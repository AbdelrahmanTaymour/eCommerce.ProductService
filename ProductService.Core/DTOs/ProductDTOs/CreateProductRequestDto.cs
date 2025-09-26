namespace ProductService.Core.DTOs.ProductDTOs;

public record CreateProductRequestDto(string Name, string? Description, decimal Price, int Stock, int CategoryId);