using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Reports.GenerateClientsReport;

public class GenerateClientsReportQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GenerateClientsReportQueryHandler> logger) : IHandler<GenerateClientsReportQuery, ClientsReportDTO[]>
{
    public async Task<IResultDTO<ClientsReportDTO[]>> HandleAsync(GenerateClientsReportQuery request,
        HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação do relatório de clientes.");

        var query = dbContext.Clients
            .Include(x => x.Partner)
            .AsNoTracking();

        if (request.FromDate.HasValue)
            query = query.Where(x =>
                x.EcacExpirationDate >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.EcacExpirationDate <= request.ToDate);
        
        if (request.PartnerId != null)
            query = query.Where(x => x.PartnerId == request.PartnerId);

        var clients = await query.ToListAsync(cancellationToken);

        if (!clients.Any())
            return ResultDTO<ClientsReportDTO[]>.AsFailure(new FailureDTO(404, "Nenhum cliente encontrado"));

        var result = clients.Select(x => new ClientsReportDTO(x.TradeName, x.LegalName, x.Cnpj.Value, x.PartnerId,
            x.Partner.LegalName, x.EcacExpirationDate, x.ExpirationStatus)).ToArray();

        return ResultDTO<ClientsReportDTO[]>.AsSuccess(result);
    }
}