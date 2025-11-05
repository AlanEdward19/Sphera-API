using Asp.Versioning;
using Sphera.API.Filters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sphera.API.Configuration;

/// <summary>
/// Provides extension methods for configuring API endpoints and related services in an ASP.NET Core application.
/// </summary>
/// <remarks>This class includes methods to set up endpoint routing, controller mapping, API versioning, health
/// checks, and JSON serialization options. Use these methods during application startup to ensure endpoints and related
/// services are properly configured for your web API.</remarks>
public static class EndpointsConfiguration
{
    public static IServiceCollection ConfigureEndpoints(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        services.AddScoped<GlobalExceptionFilter>();

        services.AddControllers(options =>
        {
            options.Filters.AddService<GlobalExceptionFilter>();
        })
            .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();

        return services;
    }
}