using ProductService.Core.DTOs.ExceptionsDTOs;
using ProductService.Core.DTOs.ProductDTOs;
using ProductService.Core.ServiceContracts;
using ProductService.Core.Validators;

namespace eCommerce.ProductService.APIEndpoints;

/// <summary>
/// A static class for defining and mapping Product-related API endpoints within the application.
/// </summary>
public static class ProductApiEndpoints
{
    /// <summary>
    /// Configures and maps all the product-related API endpoints to the specified endpoint route builder.
    /// </summary>
    /// <param name="endpoints">
    /// The <see cref="IEndpointRouteBuilder"/> used to define the routes for the product service endpoints.
    /// </param>
    public static void MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/products")
            .WithTags("Products")
            .WithOpenApi();

        // POST /api/products
        group.MapPost("", async (CreateProductRequestDto request, IProductService productService) =>
        {
            var response = await productService.CreateProductAsync(request);
            return Results.CreatedAtRoute("GetProductById", new { id = response.Id }, response);
        })
        .WithName("CreateProduct")
        .WithSummary("Create a new product")
        .WithDescription("Creates a new product with the specified details including name, description, price, stock, and category.")
        .WithValidation<CreateProductRequestDto>()
        .Accepts<CreateProductRequestDto>("application/json")
        .Produces<ProductResponseDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces<ExceptionResponseDto>(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status409Conflict)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/products/{id}
        group.MapGet("{id:guid}", async (Guid id, IProductService productService) =>
        {
            var response = await productService.GetProductByIdAsync(id);
            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .WithSummary("Get product by ID")
        .WithDescription("Retrieves a product by its unique identifier.")
        .Produces<ProductResponseDto>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/products
        group.MapGet("", async (IProductService productService) =>
        {
            var response = await productService.GetAllProductsAsync();
            return Results.Ok(response);
        })
        .WithName("GetAllProducts")
        .WithSummary("Get all products")
        .WithDescription("Retrieves all products.")
        .Produces<IEnumerable<ProductResponseDto>>()
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/products/category/{categoryId}
        group.MapGet("category/{categoryId:int}", async (int categoryId, IProductService productService) =>
        {
            var response = await productService.GetProductsByCategoryAsync(categoryId);
            return Results.Ok(response);
        })
        .WithName("GetProductsByCategory")
        .WithSummary("Get products by category")
        .WithDescription("Retrieves all products in a specific category.")
        .Produces<IEnumerable<ProductResponseDto>>()
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/products/price-range
        group.MapGet("price-range", async (decimal minPrice, decimal maxPrice, IProductService productService) =>
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                return Results.BadRequest("Invalid price range parameters.");
            }

            var response = await productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            return Results.Ok(response);
        })
        .WithName("GetProductsByPriceRange")
        .WithSummary("Get products by price range")
        .WithDescription("Retrieves products within a specified price range.")
        .Produces<IEnumerable<ProductResponseDto>>()
        .Produces<ExceptionResponseDto>(StatusCodes.Status400BadRequest)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/products/search
        group.MapGet("search", async (string searchTerm, IProductService productService) =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Results.BadRequest("Search term cannot be empty.");
            }

            var response = await productService.SearchProductsAsync(searchTerm);
            return Results.Ok(response);
        })
        .WithName("SearchProducts")
        .WithSummary("Search products")
        .WithDescription("Searches for products by name or description.")
        .Produces<IEnumerable<ProductResponseDto>>()
        .Produces<ExceptionResponseDto>(StatusCodes.Status400BadRequest)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // PUT /api/products/{id}
        group.MapPut("{id:guid}", async (Guid id, UpdateProductRequestDto request, IProductService productService) =>
        {
            var response = await productService.UpdateProductAsync(id, request);
            return Results.Ok(response);
        })
        .WithName("UpdateProduct")
        .WithSummary("Update a product")
        .WithDescription("Updates an existing product with new information.")
        .WithValidation<UpdateProductRequestDto>()
        .Accepts<UpdateProductRequestDto>("application/json")
        .Produces<ProductResponseDto>()
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status409Conflict)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // PATCH /api/products/{id}/stock
        group.MapPatch("{id:guid}/stock", async (Guid id, UpdateStockRequestDto request, IProductService productService) =>
        {
            var updated = await productService.UpdateProductStockAsync(id, request.NewStock);
            return updated ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateProductStock")
        .WithSummary("Update product stock")
        .WithDescription("Updates the stock quantity of a specific product.")
        .WithValidation<UpdateStockRequestDto>()
        .Accepts<UpdateStockRequestDto>("application/json")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // DELETE /api/products/{id}
        group.MapDelete("{id:guid}", async (Guid id, IProductService productService) =>
        {
            var deleted = await productService.DeleteProductAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithSummary("Delete a product")
        .WithDescription("Deletes a product.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);
    }
}