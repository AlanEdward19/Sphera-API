using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;
using Sphera.API.Users.GetUsers;
using Sphera.API.Users.UpdateUser;

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
        services.AddScoped<IHandler<GetUsersQuery, IEnumerable<UserDTO>>, GetUsersQueryHandler>();
        services.AddScoped<IHandler<UpdateUserCommand, UserDTO>, UpdateUserCommandHandler>();

        return services;
    }
}