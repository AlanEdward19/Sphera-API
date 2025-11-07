using Sphera.API.Clients;
using Sphera.API.External;
using Sphera.API.Partners;
using Sphera.API.Users;
using Sphera.API.Roles;

namespace Sphera.API.Configurations;

/// <summary>
/// Provides extension methods for registering external services and client-related
/// dependencies into a service collection for dependency injection.
/// </summary>
/// <remarks>This class is intended to be used during application startup to centralize the registration of
/// dependencies required by the application. All methods are static and designed to be called as part of the dependency
/// injection setup process.</remarks>
public static class IoC
{
    /// <summary>
    /// Registers external services and client-related dependencies into the specified service
    /// collection for dependency injection.
    /// </summary>
    /// <remarks>This method is intended to be called during application startup to ensure all necessary
    /// dependencies are available for injection. The method chains multiple configuration steps to set up external
    /// services, endpoints, and client-related dependencies.</remarks>
    /// <param name="services">The service collection to which the dependencies will be added. Must not be null.</param>
    /// <param name="configuration">The application configuration used to configure external services and dependencies. Must not be null.</param>
    /// <returns>The same instance of <see cref="IServiceCollection"/> with the required dependencies registered.</returns>
    public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddExernal(configuration)
            .ConfigureClientsRelatedDependencies()
            .ConfigurePartnersRelatedDependencies()
            .ConfigureUsersRelatedDependencies()
            .ConfigureRolesRelatedDependencies();

        return services;
    }
}
