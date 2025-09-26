using ProductService.Core.DTOs.ProductDTOs;

namespace ProductService.Core.ServiceContracts;

public interface IProductService
{
    Task<ProductResponseDto> CreateProductAsync(CreateProductRequestDto request);
    Task<ProductResponseDto> GetProductByIdAsync(Guid id);
    Task<ProductResponseDto> GetProductByNameAsync(string name);
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductResponseDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string searchTerm);
    Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductRequestDto request);
    Task<bool> DeleteProductAsync(Guid id);
    Task<bool> UpdateProductStockAsync(Guid id, int newStock);
}