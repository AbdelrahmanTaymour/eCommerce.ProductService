using Microsoft.Extensions.Logging;
using ProductService.Core.DTOs.CategoryDTOs;
using ProductService.Core.Exceptions.ClientErrors;
using ProductService.Core.Exceptions.ServerErrors;
using ProductService.Core.Mappers;
using ProductService.Core.RepositoryContracts;
using ProductService.Core.ServiceContracts;

namespace ProductService.Core.Services;

public class CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger) : ICategoryService
{
    /// <inheritdoc />
    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request)
    {
        // Check if a category name already exists
        var existingCategory = await categoryRepository.ExistsByNameAsync(request.Name);
        if (existingCategory)
        {
            logger.LogWarning("Category creation attempt with existing name: {CategoryName}", request.Name);
            throw new ConflictException("Category name already exists");
        }

        // Create a new category entity
        var category = request.MapToCategoryEntity();
        var createdCategory = await categoryRepository.AddAsync(category);

        logger.LogInformation("Category created successfully: {CategoryId}", createdCategory.Id);
        return createdCategory.MapToCategoryDto();
    }

    /// <inheritdoc />
    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if(category is null)
            throw new NotFoundException($"Category with id '{id}' was not found");
        
        return category.MapToCategoryDto();
    }

    /// <inheritdoc />
    public async Task<CategoryDto> GetCategoryByNameAsync(string name)
    {
        var category = await categoryRepository.GetByNameAsync(name);
        if(category is null)
            throw new NotFoundException($"Category with name '{name}' was not found");
        
        return category.MapToCategoryDto();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        return categories.Select(c => c.MapToCategoryDto());
    }

    /// <inheritdoc />
    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request)
    {
        // Check if a category exists
        var existingCategory = await categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            logger.LogWarning("Category update attempt for non-existent category: {CategoryId}", id);
            throw new NotFoundException($"Category with id '{id}' was not found");
        }

        // Check if the new name conflicts with another category
        var categoryWithSameName = await categoryRepository.GetByNameAsync(request.Name);
        if (categoryWithSameName != null && categoryWithSameName.Id != id)
        {
            logger.LogWarning("Category update attempt with conflicting name: {CategoryName}", request.Name);
            throw new ConflictException("Category name already exists");
        }

        // Update category
        existingCategory.Name = request.Name;
        var updatedCategory = await categoryRepository.UpdateAsync(existingCategory);
        if(updatedCategory == null)
            throw new ServiceUnavailableException($"Unexpectedly failed updating Category with id '{id}'");
        
        logger.LogInformation("Category updated successfully: {CategoryId}", id);
        return updatedCategory.MapToCategoryDto();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var exists = await categoryRepository.ExistsByIdAsync(id);
        if (!exists)
        {
            logger.LogWarning("Category deletion attempt for non-existent category: {CategoryId}", id);
            return false;
        }

        var deleted = await categoryRepository.DeleteAsync(id);
        if (deleted)
        {
            logger.LogInformation("Category deleted successfully: {CategoryId}", id);
        }

        return deleted;
    }
}
