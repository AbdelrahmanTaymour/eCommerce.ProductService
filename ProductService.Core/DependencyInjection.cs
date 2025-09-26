using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Core.ServiceContracts;
using ProductService.Core.Services;
using ProductService.Core.Validators.CategoryValidators;

namespace ProductService.Core;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the Core layer services and abstractions used by the application.
    /// </summary>
    /// <remarks>
    /// This method is responsible for registering domain-level services such as validators,
    /// business rules, policies, or any other cross-cutting concerns that belong to the Core domain.
    /// Typically, the Core layer contains only pure business logic and abstractions, but if there are
    /// any services to be registered, this is the place.
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which Core services will be added.
    /// </param>
    /// <param name="configuration"></param>
    /// <returns>
    /// The modified <see cref="IServiceCollection"/> with registered Core services.
    /// </returns>
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        // FluentValidations
        services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();
        
        // Services
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IProductService, Services.ProductService>();
        
        return services;
    }
}