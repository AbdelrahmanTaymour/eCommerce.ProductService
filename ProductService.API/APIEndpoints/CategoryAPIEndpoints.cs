using ProductService.Core.DTOs.CategoryDTOs;
using ProductService.Core.DTOs.ExceptionsDTOs;
using ProductService.Core.ServiceContracts;
using ProductService.Core.Validators;

namespace eCommerce.ProductService.APIEndpoints;

/// <summary>
/// Provides extension methods to map category-related API endpoints in the application.
/// </summary>
/// <remarks>
/// The <see cref="CategoryApiEndpoints"/> class is a static utility class designed to define and
/// map API endpoints for managing product categories. These endpoints include functionalities
/// such as creating, retrieving, updating, and deleting categories. It supports integration with
/// ASP.NET Core minimal APIs through the use of an endpoint route builder.
/// </remarks>
/// <example>
/// Use the <see cref="MapCategoryEndpoints"/> method to register endpoints for categories
/// to an <see cref="IEndpointRouteBuilder"/> instance. Routes are defined under the base
/// path "api/categories".
/// </example>
public static class CategoryApiEndpoints
{
    /// <summary>
    /// Configures and maps API endpoints for category-related operations within the application.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> instance used to define the category API routes.</param>
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/categories")
            .WithTags("Categories")
            .WithOpenApi();

        // POST /api/categories
        group.MapPost("", async (CreateCategoryRequestDto request, ICategoryService categoryService) =>
        {
            var response = await categoryService.CreateCategoryAsync(request);
            return Results.CreatedAtRoute("GetCategoryById", new { id = response.Id }, response);
        })
        .WithName("CreateCategory")
        .WithSummary("Create a new category")
        .WithDescription("Creates a new product category with the specified name.")
        .WithValidation<CreateCategoryRequestDto>()
        .Accepts<CreateCategoryRequestDto>("application/json")
        .Produces<CategoryDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces<ExceptionResponseDto>(StatusCodes.Status409Conflict)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/categories/{id}
        group.MapGet("{id:int}", async (int id, ICategoryService categoryService) =>
        {
            var response = await categoryService.GetCategoryByIdAsync(id);
            return Results.Ok(response);
        })
        .WithName("GetCategoryById")
        .WithSummary("Get category by ID")
        .WithDescription("Retrieves a category by its unique identifier.")
        .Produces<CategoryDto>()
        .Produces<ExceptionResponseDto>(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // GET /api/categories
        group.MapGet("", async (ICategoryService categoryService) =>
        {
            var response = await categoryService.GetAllCategoriesAsync();
            return Results.Ok(response);
        })
        .WithName("GetAllCategories")
        .WithSummary("Get all categories")
        .WithDescription("Retrieves all categories.")
        .Produces<IEnumerable<CategoryDto>>()
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // PUT /api/categories/{id}
        group.MapPut("{id:int}", async (int id, UpdateCategoryRequestDto request, ICategoryService categoryService) =>
        {
            var response = await categoryService.UpdateCategoryAsync(id, request);
            return Results.Ok(response);
        })
        .WithName("UpdateCategory")
        .WithSummary("Update a category")
        .WithDescription("Updates an existing category with new information.")
        .WithValidation<UpdateCategoryRequestDto>()
        .Accepts<UpdateCategoryRequestDto>("application/json")
        .Produces<CategoryDto>()
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status409Conflict)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);

        // DELETE /api/categories/{id}
        group.MapDelete("{id:int}", async (int id, ICategoryService categoryService) =>
        {
            var deleted = await categoryService.DeleteCategoryAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCategory")
        .WithSummary("Delete a category")
        .WithDescription("Deletes a category.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ExceptionResponseDto>(StatusCodes.Status500InternalServerError);
    }
}
