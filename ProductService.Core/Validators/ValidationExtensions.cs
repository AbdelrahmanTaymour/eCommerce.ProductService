using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ProductService.Core.Validators;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}