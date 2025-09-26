using Microsoft.Extensions.Logging;
using ProductService.Core.DTOs.ProductDTOs;
using ProductService.Core.Entities;
using ProductService.Core.Exceptions.ClientErrors;
using ProductService.Core.Mappers;
using ProductService.Core.RepositoryContracts;
using ProductService.Core.ServiceContracts;

namespace ProductService.Core.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ILogger<ProductService> logger) : IProductService
{
    /// <inheritdoc />
    public async Task<ProductResponseDto> CreateProductAsync(CreateProductRequestDto request)
    {
        // Verify category exists
        var categoryExists = await categoryRepository.ExistsByIdAsync(request.CategoryId);
        if (!categoryExists)
        {
            logger.LogWarning("Product creation attempt with non-existent category: {CategoryId}", request.CategoryId);
            throw new NotFoundException("Category not found");
        }

        // Check if product name already exists
        var existingProduct = await productRepository.ExistsByNameAsync(request.Name);
        if (existingProduct)
        {
            logger.LogWarning("Product creation attempt with existing name: {ProductName}", request.Name);
            throw new ConflictException("Product name already exists");
        }

        // Create new product entity
        var product = request.MapToProduct();
        var createdProduct = await productRepository.AddAsync(product);

        logger.LogInformation("Product created successfully: {ProductId}", createdProduct.Id);
        return await GetProductResponseWithCategoryName(createdProduct);
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);
        return product != null ? await GetProductResponseWithCategoryName(product) : null;
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto?> GetProductByNameAsync(string name)
    {
        var product = await productRepository.GetByNameAsync(name);
        return product != null ? await GetProductResponseWithCategoryName(product) : null;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await productRepository.GetAllAsync();
        var productResponses = new List<ProductResponseDto>();

        foreach (var product in products)
        {
            productResponses.Add(await GetProductResponseWithCategoryName(product));
        }

        return productResponses;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await productRepository.GetByCategoryIdAsync(categoryId);
        var productResponses = new List<ProductResponseDto>();

        foreach (var product in products)
        {
            productResponses.Add(await GetProductResponseWithCategoryName(product));
        }

        return productResponses;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductResponseDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var products = await productRepository.GetByPriceRangeAsync(minPrice, maxPrice);
        var productResponses = new List<ProductResponseDto>();

        foreach (var product in products)
        {
            productResponses.Add(await GetProductResponseWithCategoryName(product));
        }

        return productResponses;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await productRepository.SearchAsync(searchTerm);
        var productResponses = new List<ProductResponseDto>();

        foreach (var product in products)
        {
            productResponses.Add(await GetProductResponseWithCategoryName(product));
        }

        return productResponses;
    }

    /// <inheritdoc />
    public async Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductRequestDto request)
    {
        // Check if the product exists
        var existingProduct = await productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            logger.LogWarning("Product update attempt for non-existent product: {ProductId}", id);
            throw new NotFoundException($"Product with id '{id}' was not found");       
        }

        // Verify category exists
        var categoryExists = await categoryRepository.ExistsByIdAsync(request.CategoryId);
        if (!categoryExists)
        {
            logger.LogWarning("Product update attempt with non-existent category: {CategoryId}", request.CategoryId);
            throw new NotFoundException($"Category with id '{request.CategoryId}' was not found");
        }

        // Check if the new name conflicts with another product
        var productWithSameName = await productRepository.GetByNameAsync(request.Name);
        if (productWithSameName != null && productWithSameName.Id != id)
        {
            logger.LogWarning("Product update attempt with conflicting name: {ProductName}", request.Name);
            throw new ConflictException("Product name already exists");
        }

        // Update product
        existingProduct.Name = request.Name;
        existingProduct.Description = request.Description;
        existingProduct.Price = request.Price;
        existingProduct.Stock = request.Stock;
        existingProduct.CategoryId = request.CategoryId;

        var updatedProduct = await productRepository.UpdateAsync(existingProduct);

        if (updatedProduct != null)
        {
            logger.LogInformation("Product updated successfully: {ProductId}", id);
            return await GetProductResponseWithCategoryName(updatedProduct);
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            logger.LogWarning("Product deletion attempt for non-existent product: {ProductId}", id);
            return false;
        }

        var deleted = await productRepository.DeleteAsync(id);
        if (deleted)
        {
            logger.LogInformation("Product deleted successfully: {ProductId}", id);
        }

        return deleted;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProductStockAsync(Guid id, int newStock)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            logger.LogWarning("Product stock update attempt for non-existent product: {ProductId}", id);
            return false;
        }

        var updated = await productRepository.UpdateStockAsync(id, newStock);
        if (updated)
        {
            logger.LogInformation("Product stock updated successfully: {ProductId}, New Stock: {Stock}", id, newStock);
        }

        return updated;
    }

    private async Task<ProductResponseDto> GetProductResponseWithCategoryName(Product product)
    {
        var category = await categoryRepository.GetByIdAsync(product.CategoryId);
        var response = product.MapToProductResponse();
        response.CategoryName = category?.Name;
        return response;
    }
}
