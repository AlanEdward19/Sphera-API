using Sphera.API.Billing.Billets.CreateBillet;
using Sphera.API.Billing.Billets.DeleteBillet;
using Sphera.API.Billing.Billets.GetBilletById;
using Sphera.API.Billing.Billets.ListBillets;
using Sphera.API.Billing.Billets.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Billets;

public static class BilletsModule
{
    public static IServiceCollection ConfigureBilletsRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<ListBilletsQuery, IReadOnlyCollection<BilletDTO>>, ListBilletsQueryHandler>();
        services.AddScoped<IHandler<GetBilletByIdQuery, BilletDTO>, GetBilletByIdQueryHandler>();
        services.AddScoped<IHandler<CreateBilletCommand, BilletDTO>, CreateBilletCommandHandler>();
        services.AddScoped<IHandler<DeleteBilletCommand, bool>, DeleteBilletCommandHandler>();

        return services;
    }
}