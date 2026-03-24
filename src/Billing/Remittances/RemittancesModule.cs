using Sphera.API.Billing.Remittances.CreateRemittance;
using Sphera.API.Billing.Remittances.DeleteRemittance;
using Sphera.API.Billing.Remittances.DTOs;
using Sphera.API.Billing.Remittances.GetRemittanceById;
using Sphera.API.Billing.Remittances.ListRemittances;
using Sphera.API.Billing.Remittances.SubmitRemittance;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Billing.Remittances.GenerateRemittanceFile;
using Sphera.API.Billing.Remittances.DownloadRemittanceFile;

namespace Sphera.API.Billing.Remittances;

public static class RemittancesModule
{
    public static IServiceCollection ConfigureRemittancesRelatedDependencies(this IServiceCollection services)
    {
        services.ConfigureHandlers();
        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<ListRemittancesQuery, IReadOnlyCollection<RemittanceDTO>>, ListRemittancesQueryHandler>();
        services.AddScoped<IHandler<GetRemittanceByIdQuery, RemittanceDTO>, GetRemittanceByIdQueryHandler>();
        services.AddScoped<IHandler<CreateRemittanceCommand, RemittanceDTO>, CreateRemittanceCommandHandler>();
        services.AddScoped<IHandler<SubmitRemittanceCommand, RemittanceDTO>, SubmitRemittanceCommandHandler>();
        services.AddScoped<IHandler<DeleteRemittanceCommand, bool>, DeleteRemittanceCommandHandler>();
        services.AddScoped<IHandler<GenerateRemittanceFileCommand, bool>, GenerateRemittanceFileCommandHandler>();
        services.AddScoped<IHandler<DownloadRemittanceFileCommand, (Stream, string)>, DownloadRemittanceFileCommandHandler>();
        return services;
    }
}
