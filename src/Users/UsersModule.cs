using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users;

public static class UsersModule
{
    public static IServiceCollection ConfigureUsersRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }
    
    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateUserCommand, UserDTO>, CreateUserCommandHandler>();

        return services;
    }
}