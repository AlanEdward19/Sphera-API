using Sphera.API.Auditory.GetAuditories;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Auditory;

public static class AuditoryModule
{
    public static IServiceCollection ConfigureAuditoryRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }
    
    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<GetAuditoriesQuery, IEnumerable<AuditoryDTO>>, GetAuditoriesQueryHandler>();
        return services;
    }
}