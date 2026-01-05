using Sphera.API.Reports.GenerateClientsReport;
using Sphera.API.Reports.GenerateFilesReport;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Reports;

public static class ReportsModule
{
    public static IServiceCollection ConfigureReportsRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();
        
        return services;
    }
    
    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services
            .AddScoped<IHandler<GenerateClientsReportQuery, ClientsReportDTO[]>, GenerateClientsReportQueryHandler>();
        services
            .AddScoped<IHandler<GenerateFilesReportQuery, FilesReportDTO[]>, GenerateFilesReportQueryHandler>();
        
        return services;
    }
}