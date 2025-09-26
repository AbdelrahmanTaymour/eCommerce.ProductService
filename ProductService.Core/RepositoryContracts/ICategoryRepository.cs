using ProductService.Core.Entities;

namespace ProductService.Core.RepositoryContracts;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByNameAsync(string name);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> AddAsync(Category category);
    Task<Category?> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByIdAsync(int id);
}