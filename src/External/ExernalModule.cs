using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;

namespace Sphera.API.External;

public static class ExernalModule
{
    public static IServiceCollection AddExernal(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddData(configuration);
        return services;
    }
    
    private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SpheraDbContext>(o =>
            o.UseSqlServer(configuration.GetConnectionString("Database")));

        return services;
    }
}