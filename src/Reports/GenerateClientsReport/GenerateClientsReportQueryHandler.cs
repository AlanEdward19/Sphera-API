using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Reports.GenerateClientsReport;

/// <summary>
/// Handles the query for generating a clients report.
/// This class is responsible for processing the <see cref="GenerateClientsReportQuery"/>
/// and returning an array of <see cref="ClientsReportDTO"/> containing the clients data
/// based on the query criteria.
/// </summary>
/// <remarks>
/// This handler uses <see cref="SpheraDbContext"/> to access the database and fetch clients data.
/// It logs the processing steps, applies query filters based on the request parameters,
/// and returns results or appropriate failure responses.
/// </remarks>
public class GenerateClientsReportQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GenerateClientsReportQueryHandler> logger) : IHandler<GenerateClientsReportQuery, ClientsReportDTO[]>
{
    /// <summary>
    /// Handles the processing of a GenerateClientsReportQuery to create a report of clients
    /// based on specified filters, such as date range, status or partner ID.
    /// </summary>
    /// <param name="request">The request object containing query parameters for generating the clients report.</param>
    /// <param name="context">The HTTP context for accessing metadata or request details.</param>
    /// <param name="cancellationToken">A cancellation token to propagate notification of request cancellation.</param>
    /// <returns>A result object containing an array of <see cref="ClientsReportDTO"/> if successful, or a failure result if no clients are found.</returns>
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

        if (request.PaymentStatus.HasValue)
            query = query.Where(x => x.PaymentStatus == request.PaymentStatus.Value);

        var clients = await query.ToListAsync(cancellationToken);

        if (!clients.Any())
            return ResultDTO<ClientsReportDTO[]>.AsFailure(new FailureDTO(404, "Nenhum cliente encontrado"));
        
        if (request.Status.HasValue)
            clients = clients.Where(x => x.ExpirationStatus == request.Status.Value).ToList();

        var result = clients.Select(x => new ClientsReportDTO(x.TradeName, x.LegalName, x.Cnpj.Value, x.PartnerId,
            x.Partner.LegalName, x.EcacExpirationDate, x.ExpirationStatus, x.PaymentStatus)).ToArray();

        return ResultDTO<ClientsReportDTO[]>.AsSuccess(result);
    }
}