using ProductService.Core.DTOs.CategoryDTOs;

namespace ProductService.Core.ServiceContracts;

public interface ICategoryService
{
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request);
    Task<CategoryDto> GetCategoryByIdAsync(int id);
    Task<CategoryDto> GetCategoryByNameAsync(string name);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request);
    Task<bool> DeleteCategoryAsync(int id);
}