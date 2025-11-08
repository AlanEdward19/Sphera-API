using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sphera.API.Auths.DTOs;
using Sphera.API.Auths.Login;
using Sphera.API.Auths.RefreshToken;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Services;

namespace Sphera.API.Auths;

public static class AuthsModule
{
    public static IServiceCollection ConfigureAuthsRelatedDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureHandlers();
        services.AddAuth(configuration);
        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<LoginCommand, LoginDTO>, LoginCommandHandler>();
        services.AddScoped<IHandler<RefreshTokenCommand, RefreshTokenDTO>, RefreshTokenCommandHandle>();
        return services;
    }
    
    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthUtilityService, AuthUtilityService>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty))
                    
                };
            });
        return services;
    }
}