using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ClientErrors;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public sealed class ValidationException : ClientErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

     public Dictionary<string, string[]> ValidationErrors { get; }

     private static string DefaultMessage => "One or more validation errors occurred.";

     public ValidationException(string message)
         : base(message)
     {
         ValidationErrors = new Dictionary<string, string[]>();
     }

     public ValidationException(Dictionary<string, string[]> validationErrors)
         : base(DefaultMessage, validationErrors)
     {
         ValidationErrors = validationErrors;
     }

     public ValidationException(string field, string error)
         : base($"Validation failed for {field}", new Dictionary<string, string[]>
         {
             { field, new[] { error } }
         })
     {
         ValidationErrors = Details as Dictionary<string, string[]> ?? new Dictionary<string, string[]>();
     }

     /// <summary>
     /// Creates ValidationException from FluentValidation ValidationResult.
     /// </summary>
     public ValidationException(FluentValidation.Results.ValidationResult validationResult)
         : base(DefaultMessage, ToDictionary(validationResult.Errors))
     {
         ValidationErrors = Details as Dictionary<string, string[]>
                            ?? new Dictionary<string, string[]>();
     }

     /// <summary>
     /// Creates ValidationException from FluentValidation ValidationException.
     /// </summary>
     public ValidationException(FluentValidation.ValidationException fluentValidationException)
         : base(DefaultMessage, ToDictionary(fluentValidationException.Errors))
     {
         ValidationErrors = Details as Dictionary<string, string[]>
                            ?? new Dictionary<string, string[]>();
     }

     /// <summary>
     /// Converts a collection of FluentValidation.ValidationFailure objects into a dictionary,
     /// where the keys are property names and the values are arrays of error messages.
     /// </summary>
     /// <param name="errors">The collection of validation errors to convert.</param>
     /// <returns>A dictionary containing property names as keys and arrays of corresponding error messages as values.</returns>
     private static Dictionary<string, string[]> ToDictionary(
         IEnumerable<FluentValidation.Results.ValidationFailure> errors)
     {
         return errors
             .GroupBy(x => x.PropertyName)
             .ToDictionary(
                 g => g.Key,
                 g => g.Select(x => x.ErrorMessage).ToArray()
             );
     }
}