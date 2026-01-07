using Sphera.API.Billing.BilletConfigurations.CreateBilletConfiguration;
using Sphera.API.Billing.BilletConfigurations.DeleteBilletConfiguration;
using Sphera.API.Billing.BilletConfigurations.DTOs;
using Sphera.API.Billing.BilletConfigurations.GetBilletConfigurationById;
using Sphera.API.Billing.BilletConfigurations.ListBilletConfigurations;
using Sphera.API.Billing.BilletConfigurations.UpdateBilletConfiguration;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.BilletConfigurations;

public static class BilletConfigurationsModule
{
    public static IServiceCollection ConfigureBilletConfigurationsRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<ListBilletConfigurationsQuery, IReadOnlyCollection<BilletConfigurationDTO>>, ListBilletConfigurationsQueryHandler>();
        services.AddScoped<IHandler<GetBilletConfigurationByIdQuery, BilletConfigurationDTO>, GetBilletConfigurationByIdQueryHandler>();
        services.AddScoped<IHandler<CreateBilletConfigurationCommand, BilletConfigurationDTO>, CreateBilletConfigurationCommandHandler>();
        services.AddScoped<IHandler<UpdateBilletConfigurationCommand, BilletConfigurationDTO>, UpdateBilletConfigurationCommandHandler>();
        services.AddScoped<IHandler<DeleteBilletConfigurationCommand, bool>, DeleteBilletConfigurationCommandHandler>();

        return services;
    }
}