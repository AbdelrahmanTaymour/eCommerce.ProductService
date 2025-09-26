using Dapper;
using Microsoft.Extensions.Logging;
using ProductService.Core.Entities;
using ProductService.Core.Exceptions.ServerErrors;
using ProductService.Core.RepositoryContracts;
using ProductService.Infrastructure.DbContexts;

namespace ProductService.Infrastructure.Repositories;

public class CategoryRepository(DapperDbContext dbContext, ILogger<CategoryRepository> logger) : ICategoryRepository
{
    /// <inheritdoc/>
    public async Task<Category?> GetByIdAsync(int id)
    {
        const string sql = """
                               SELECT id, name, created_at, updated_at
                               FROM public.categories 
                               WHERE id = @Id
                           """;

        try
        {
            return await dbContext.Connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category by ID: {CategoryId}", id);
            throw new DatabaseException("Failed to retrieve category", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Category?> GetByNameAsync(string name)
    {
        const string sql = """
                                SELECT id, name, created_at, updated_at
                                FROM public.categories 
                                WHERE name = @Name
                           """;

        try
        {
            return await dbContext.Connection.QueryFirstOrDefaultAsync<Category>(sql, new { Name = name });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category by name: {CategoryName}", name);
            throw new DatabaseException("Failed to retrieve category by name", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        const string sql = """

                                       SELECT id, name, created_at, updated_at
                                       FROM public.categories 
                                       ORDER BY name
                           """;

        try
        {
            return await dbContext.Connection.QueryAsync<Category>(sql);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all categories");
            throw new DatabaseException("Failed to retrieve categories", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Category> AddAsync(Category category)
    {
        const string sql = """

                                       INSERT INTO public.categories (name) 
                                       VALUES (@Name)
                                       RETURNING id, name, created_at, updated_at
                           """;

        try
        {
            return await dbContext.Connection.QuerySingleAsync<Category>(sql, category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding new category: {CategoryName}", category.Name);
            throw new DatabaseException("Failed to add new category", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Category?> UpdateAsync(Category category)
    {
        const string sql = """
                                       UPDATE public.categories 
                                       SET name = @Name, updated_at = @UpdatedAt
                                       WHERE id = @Id
                                       RETURNING id, name, created_at, updated_at
                           """;

        try
        {
            return await dbContext.Connection.QueryFirstOrDefaultAsync<Category>(sql, new
            {
                category.Id,
                category.Name,
                UpdatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating category: {CategoryId}", category.Id);
            throw new DatabaseException("Failed to update category", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM public.categories WHERE id = @Id";

        try
        {
            var affectedRows = await dbContext.Connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting category: {CategoryId}", id);
            throw new DatabaseException("Failed to delete category", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        const string sql = """
                                       SELECT EXISTS(
                                       SELECT 1 FROM public.categories
                                       WHERE name = @Name)
                           """;

        try
        {
            return await dbContext.Connection.ExecuteScalarAsync<bool>(sql, new { Name = name });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category exists by name: {CategoryName}", name);
            throw new DatabaseException("Failed to check category existence by name", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByIdAsync(int id)
    {
        const string sql = """
                                       SELECT EXISTS(
                                       SELECT 1 FROM public.categories
                                       WHERE id = @Id)
                           """;

        try
        {
            return await dbContext.Connection.ExecuteScalarAsync<bool>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category exists by ID: {CategoryId}", id);
            throw new DatabaseException("Failed to check category existence by ID", ex);
        }
    }
}
