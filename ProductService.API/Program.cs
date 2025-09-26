using System.Reflection;
using System.Text.Json.Serialization;
using eCommerce.ProductService.APIEndpoints;
using eCommerce.ProductService.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using ProductService.Core;
using ProductService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Custom Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore(builder.Configuration);

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
 {
     options.SwaggerDoc("v1", new OpenApiInfo
     {
         Title = "E-Commerce User Service API",
         Version = "v1",
         Description = "API documentation for the User Service"
     });

     // Include XML comments
     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
     if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);

     // XML Comments for other referenced projects
     var additionalXmlFiles = new[] { "eCommerce.Core.xml" };
     foreach (var xml in additionalXmlFiles)
     {
         var fullPath = Path.Combine(AppContext.BaseDirectory, xml);
         if (File.Exists(fullPath)) options.IncludeXmlComments(fullPath);
     }
 }
);


// Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => { options.FormatterName = ConsoleFormatterNames.Simple; });
builder.Services.Configure<SimpleConsoleFormatterOptions>(options =>
{
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    options.IncludeScopes = false;
});

// FluentValidations
builder.Services.AddFluentValidationAutoValidation();

// Add a model binder to read values from JSON to enum
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add cors service
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:7085") // Add your frontend URLs
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Only if you need cookies/sessions
    });
});

// Add authentication/authorization services
builder.Services.AddAuthentication(/* configure JWT or scheme here */);
builder.Services.AddAuthorization();

// Build the application.
var app = builder.Build();

// Exception Handling 
app.UseMiddleware<ExceptionHandlingMiddleware>();

// CORS
app.UseCors();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Routing
app.UseRouting();

//Auth
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.Run();