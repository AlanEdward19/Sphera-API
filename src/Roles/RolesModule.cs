using Sphera.API.Roles.DTOs;
using Sphera.API.Roles.GetRoles;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Roles;

public static class RolesModule
{
    public static IServiceCollection ConfigureRolesRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<GetRolesQuery, IEnumerable<RoleDTO>>, GetRolesQueryHandler>();

        return services;
    }
}