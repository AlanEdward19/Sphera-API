using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;
using Sphera.API.Users.GetUsers;
using Sphera.API.Users.UpdateUser;
using Sphera.API.Users.ActivateUser;
using Sphera.API.Users.DeactivateUser;
using Sphera.API.Users.DeleteUser;

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
        services.AddScoped<IHandler<ActivateUserCommand, bool>, ActivateUserCommandHandler>();
        services.AddScoped<IHandler<DeactivateUserCommand, bool>, DeactivateUserCommandHandler>();
        services.AddScoped<IHandler<DeleteUserCommand, bool>, DeleteUserCommandHandler>();

        return services;
    }
}