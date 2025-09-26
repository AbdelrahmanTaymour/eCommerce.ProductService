namespace ProductService.Core.DTOs.ProductDTOs;

public record UpdateProductRequestDto(string Name, string? Description, decimal Price, int Stock, int CategoryId);