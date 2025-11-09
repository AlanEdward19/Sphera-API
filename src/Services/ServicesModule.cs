using Sphera.API.Services.ActivateService;
using Sphera.API.Services.CreateService;
using Sphera.API.Services.DeactivateService;
using Sphera.API.Services.DeleteService;
using Sphera.API.Services.GetServiceById;
using Sphera.API.Services.GetServices;
using Sphera.API.Services.UpdateService;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Services;

public static class ServicesModule
{
    public static IServiceCollection ConfigureServicesRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();
        
        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<GetServiceByIdQuery, ServiceDTO>, GetServiceByIdQueryHandler>();
        services.AddScoped<IHandler<GetServicesQuery, IEnumerable<ServiceDTO>>, GetServicesQueryHandler>();
        services.AddScoped<IHandler<ActivateServiceCommand, bool>, ActivateServiceCommandHandler>();
        services.AddScoped<IHandler<DeactivateServiceCommand, bool>, DeactivateServiceCommandHandler>();
        services.AddScoped<IHandler<CreateServiceCommand, ServiceDTO>, CreateServiceCommandHandler>();
        services.AddScoped<IHandler<DeleteServiceCommand, bool>, DeleteServiceCommandHandler>();
        services.AddScoped<IHandler<UpdateServiceCommand, ServiceDTO>, UpdateServiceCommandHandler>();

        return services;
    }
}