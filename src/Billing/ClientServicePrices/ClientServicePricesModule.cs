using Sphera.API.Billing.ClientServicePrices.CreateClientServicePrice;
using Sphera.API.Billing.ClientServicePrices.GetClientServicePriceById;
using Sphera.API.Billing.ClientServicePrices.ListClientServicePrices;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.ClientServicePrices;

public static class ClientServicePricesModule
{
    public static IServiceCollection ConfigureClientServicePricesRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateClientServicePriceCommand, ClientServicePriceDTO>, CreateClientServicePriceCommandHandler>();
        services.AddScoped<IHandler<GetClientServicePriceByIdQuery, ClientServicePriceDTO>, GetClientServicePriceByIdQueryHandler>();
        services.AddScoped<IHandler<ListClientServicePricesQuery, IReadOnlyCollection<ClientServicePriceDTO>>, ListClientServicePricesQueryHandler>();

        return services;
    }
}