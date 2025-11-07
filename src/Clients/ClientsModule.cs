using Sphera.API.Clients.ActivateClient;
using Sphera.API.Clients.CreateClient;
using Sphera.API.Clients.DeactivateClient;
using Sphera.API.Clients.DeleteClient;
using Sphera.API.Clients.DTOs;
using Sphera.API.Clients.GetClientById;
using Sphera.API.Clients.GetClients;
using Sphera.API.Clients.UpdateClient;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Clients;

/// <summary>
/// Provides extension methods for configuring client-related services and handler dependencies within an application's
/// dependency injection container.
/// </summary>
/// <remarks>Use this static class during application startup to register client service handlers and related
/// dependencies. The provided methods are intended to be called before building the service provider to ensure that
/// client command and query handlers are available for resolution via dependency injection.</remarks>
public static class ClientsModule
{
    /// <summary>
    /// Configures dependencies related to client services and handlers within the specified service collection.
    /// </summary>
    /// <remarks>This method is intended to be used during application startup to register client service
    /// handlers and related dependencies for dependency injection. It should be called before building the service
    /// provider.</remarks>
    /// <param name="services">The service collection to which client-related dependencies will be added. Cannot be null.</param>
    /// <returns>The same service collection instance with client-related dependencies configured.</returns>
    public static IServiceCollection ConfigureClientsRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    /// <summary>
    /// Registers command and query handler services for client-related operations in the dependency injection
    /// container.
    /// </summary>
    /// <remarks>This extension method adds scoped implementations for handlers managing client creation,
    /// update, deletion, and retrieval. It enables the application to resolve these handlers via dependency injection
    /// for processing client commands and queries.</remarks>
    /// <param name="services">The service collection to which the handler services will be added.</param>
    /// <returns>The same service collection instance with the handler services registered.</returns>
    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateClientCommand, ClientDTO>, CreateClientCommandHandler>();
        services.AddScoped<IHandler<UpdateClientCommand, ClientDTO>, UpdateClientCommandHandler>();
        services.AddScoped<IHandler<DeleteClientCommand, bool>, DeleteClientCommandHandler>();
        services.AddScoped<IHandler<GetClientsQuery, IEnumerable<ClientDTO>>, GetClientsQueryHandler>();
        services.AddScoped<IHandler<GetClientByIdQuery, ClientDTO>, GetClientByIdQueryHandler>();
        services.AddScoped<IHandler<ActivateClientCommand, bool>, ActivateClientCommandHandler>();
        services.AddScoped<IHandler<DeactivateClientCommand, bool>, DeactivateClientCommandHandler>();

        return services;
    }
}
