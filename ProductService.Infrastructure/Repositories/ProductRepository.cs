using Dapper;
using Microsoft.Extensions.Logging;
using ProductService.Core.Entities;
using ProductService.Core.Exceptions.ServerErrors;
using ProductService.Core.RepositoryContracts;
using ProductService.Infrastructure.DbContexts;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository(DapperDbContext dbContext, ILogger<ProductRepository> logger) : IProductRepository
{
    /// <inheritdoc/>
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        const string sql = """
                                SELECT id, name, description, price, stock, category_id, created_at, updated_at
                                FROM public.products 
                                WHERE id = @Id
                           """;

        try
        {
            return await dbContext.Connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving product by ID: {ProductId}", id);
            throw new DatabaseException("Failed to retrieve product", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Product?> GetByNameAsync(string name)
    {
        const string sql = """
                                SELECT id, name, description, price, stock, category_id, created_at, updated_at
                                FROM public.products 
                                WHERE name = @Name
                           """;

        try
        {
            return await dbContext.Connection.QueryFirstOrDefaultAsync<Product>(sql, new { Name = name });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving product by name: {ProductName}", name);
            throw new DatabaseException("Failed to retrieve product by name", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string sql = """
                                SELECT id, name, description, price, stock, category_id, created_at, updated_at
                                FROM public.products 
                                ORDER BY name
                           """;

        try
        {
            return await dbContext.Connection.QueryAsync<Product>(sql);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all products");
            throw new DatabaseException("Failed to retrieve products", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        const string sql = """
                                SELECT id, name, description, price, stock, category_id, created_at, updated_at
                                FROM public.products 
                                WHERE category_id = @CategoryId
                                ORDER BY name
                           """;

        try
        {
            return await dbContext.Connection.QueryAsync<Product>(sql, new { CategoryId = categoryId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving products by category ID: {CategoryId}", categoryId);
            throw new DatabaseException("Failed to retrieve products by category", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        const string sql = """
                                SELECT id, name, description, price, stock, category_id, created_at, updated_at
                                FROM public.products 
                                WHERE price BETWEEN @MinPrice AND @MaxPrice
                                ORDER BY price
                           """;

        try
        {
            return await dbContext.Connection.QueryAsync<Product>(sql, new { MinPrice = minPrice, MaxPrice = maxPrice });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving products by price range: {MinPrice}-{MaxPrice}", minPrice, maxPrice);
            throw new DatabaseException("Failed to retrieve products by price range", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        const string sql = """
                                SELECT id, name, description, price, stock, category_id, created_at, updated_at
                                FROM public.products 
                                WHERE name ILIKE @SearchTerm OR description ILIKE @SearchTerm
                                ORDER BY name
                           """;

        try
        {
            return await dbContext.Connection.QueryAsync<Product>(sql, new { SearchTerm = $"%{searchTerm}%" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
            throw new DatabaseException("Failed to search products", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Product> AddAsync(Product product)
    {
        const string sql = """
                                INSERT INTO public.products (name, description, price, stock, category_id) 
                                VALUES (@Name, @Description, @Price, @Stock, @CategoryId)
                                RETURNING id, name, description, price, stock, category_id, created_at, updated_at
                           """;

        try
        {
            return await dbContext.Connection.QuerySingleAsync<Product>(sql, product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding new product: {ProductName}", product.Name);
            throw new DatabaseException("Failed to add new product", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Product?> UpdateAsync(Product product)
    {
        const string sql = """
                                UPDATE public.products 
                                SET name = @Name, 
                                    description = @Description, 
                                    price = @Price, 
                                    stock = @Stock, 
                                    category_id = @CategoryId,
                                    updated_at = @UpdatedAt
                                WHERE id = @Id
                                AND NOT EXISTS (
                                    SELECT 1 
                                    FROM public.products 
                                    WHERE name = @Name AND id <> @Id
                                )
                                RETURNING id, name, description, price, stock, category_id, created_at, updated_at
                           """;

        try
        {
            return await dbContext.Connection.QueryFirstOrDefaultAsync<Product>(sql, new
            {
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.CategoryId,
                UpdatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product: {ProductId}", product.Id);
            throw new DatabaseException("Failed to update product", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM public.products WHERE id = @Id";

        try
        {
            var affectedRows = await dbContext.Connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product: {ProductId}", id);
            throw new DatabaseException("Failed to delete product", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        const string sql = """
                                SELECT EXISTS(
                                SELECT 1 FROM public.products
                                WHERE name = @Name)
                           """;

        try
        {
            return await dbContext.Connection.ExecuteScalarAsync<bool>(sql, new { Name = name });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if product exists by name: {ProductName}", name);
            throw new DatabaseException("Failed to check product existence by name", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateStockAsync(Guid id, int newStock)
    {
        const string sql = """
                                UPDATE public.products 
                                SET stock = @NewStock, updated_at = @UpdatedAt
                                WHERE id = @Id
                           """;

        try
        {
            var affectedRows = await dbContext.Connection.ExecuteAsync(sql, new
            {
                Id = id,
                NewStock = newStock,
                UpdatedAt = DateTime.UtcNow
            });

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product stock: {ProductId}", id);
            throw new DatabaseException("Failed to update product stock", ex);
        }
    }
}

