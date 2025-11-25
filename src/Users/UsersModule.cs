using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;
using Sphera.API.Users.GetUsers;
using Sphera.API.Users.UpdateUser;
using Sphera.API.Users.ActivateUser;
using Sphera.API.Users.ChangePassword;
using Sphera.API.Users.CheckFirstAccess;
using Sphera.API.Users.DeactivateUser;
using Sphera.API.Users.DeleteUser;
using Sphera.API.Users.FirstAccessPassword;

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
        services.AddScoped<IHandler<ChangePasswordCommand, bool>, ChangePasswordCommandHandler>();
        services.AddScoped<IHandler<FirstAccessPasswordCommand, bool>, FirstAccessPasswordCommandHandler>();
        services.AddScoped<IHandler<CheckFirstAccessQuery, UserDTO>, CheckFirstAccessQueryHandler>();

        return services;
    }
}