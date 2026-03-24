using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.External.Storage;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.External;

/// <summary>
/// Provides extension methods for registering external data services and managing database migrations within an
/// application's dependency injection and startup pipeline.
/// </summary>
public static class ExernalModule
{
    /// <summary>
    /// Adds external data-related services to the specified service collection using the provided configuration.
    /// </summary>
    /// <param name="services">The service collection to which the external data services will be added. Cannot be null.</param>
    /// <param name="configuration">The configuration settings used to configure the external data services. Cannot be null.</param>
    /// <returns>The same service collection instance with external data services registered.</returns>
    public static IServiceCollection AddExernal(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddData(configuration)
            .AddStorage(configuration);

        return services;
    }

    private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SpheraDbContext>(o =>
            o.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }));

        return services;
    }

    /// <summary>
    /// Applies any pending database migrations for the application's configured DbContext at startup.
    /// </summary>
    /// <remarks>This method should be called during application startup to ensure the database schema is up
    /// to date. It uses the application's service provider to resolve the DbContext and applies migrations if any are
    /// pending. If no DbContext is configured or no migrations are pending, no action is taken.</remarks>
    /// <param name="app">The application builder used to configure the application's request pipeline.</param>
    /// <returns>The same <paramref name="app"/> instance, enabling method chaining.</returns>
    /// <exception cref="Exception">Thrown if an error occurs while retrieving or applying migrations.</exception>
    public static IApplicationBuilder UpdateMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

        var context = serviceScope?.ServiceProvider.GetRequiredService<SpheraDbContext>();

        if (context != null)
        {
            try
            {
                var pendingMigrations = context.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        return app;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringStorage = configuration.GetConnectionString("Storage");
        
        services.AddKeyedScoped<IStorage>("documents", (_, _) =>
            new SpheraStorage(connectionStringStorage!, "documents"));
        services.AddKeyedScoped<IStorage>("billing", (_, _) =>
            new SpheraStorage(connectionStringStorage!, "billing"));

        return services;
    }
}