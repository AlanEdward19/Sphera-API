using Sphera.API.Partners.CreatePartner;
using Sphera.API.Partners.DeletePartner;
using Sphera.API.Partners.DTOs;
using Sphera.API.Partners.GetPartnerById;
using Sphera.API.Partners.GetPartners;
using Sphera.API.Partners.UpdatePartner;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Partners;

/// <summary>
/// Provides extension methods for configuring partner-related service dependencies and handlers within the
/// application's dependency injection container.
/// </summary>
/// <remarks>This class is intended to be used during application startup to register partner-related services and
/// handlers with the dependency injection system. All methods are static and should be called on an instance of <see
/// cref="IServiceCollection"/>.</remarks>
public static class PartnerModule
{
    /// <summary>
    /// Configures partner-related service dependencies and handlers for the application's dependency injection
    /// container.
    /// </summary>
    /// <param name="services">The service collection to which partner-related dependencies and handlers will be added. Cannot be null.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance with partner-related dependencies configured.</returns>
    public static IServiceCollection ConfigurePartnersRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();
        
        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreatePartnerCommand, PartnerDTO>, CreatePartnerCommandHandler>();
        services.AddScoped<IHandler<UpdatePartnerCommand, PartnerDTO>, UpdatePartnerCommandHandler>();
        services.AddScoped<IHandler<DeletePartnerCommand, bool>, DeletePartnerCommandHandler>();
        services.AddScoped<IHandler<GetPartnersQuery, IEnumerable<PartnerDTO>>, GetPartnersQueryHandler>();
        services.AddScoped<IHandler<GetPartnerByIdQuery, PartnerDTO>, GetPartnerByIdQueryHandler>();
        
        return services;
    }
}
