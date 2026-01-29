using Microsoft.EntityFrameworkCore;
using Sphera.API.Documents;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Reports.GenerateFilesReport;

/// <summary>
/// Handles the generation of a files report based on the specified query parameters.
/// </summary>
/// <remarks>
/// This class processes a <see cref="GenerateFilesReportQuery"/> and retrieves a list of documents
/// matching the query criteria. The results are returned as a collection of <see cref="FilesReportDTO"/> objects.
/// </remarks>
/// <param name="dbContext">
/// An instance of <see cref="SpheraDbContext"/> used to access the database.
/// </param>
/// <param name="logger">
/// An instance of <see cref="ILogger{TCategoryName}"/> used to log operational details during execution.
/// </param>
/// <seealso cref="GenerateFilesReportQuery"/>
/// <seealso cref="FilesReportDTO"/>
public class GenerateFilesReportQueryHandler(SpheraDbContext dbContext, ILogger<GenerateFilesReportQueryHandler> logger) : IHandler<GenerateFilesReportQuery, FilesReportDTO[]>
{
    /// <summary>
    /// Handles the request to generate a report of files based on the provided query parameters.
    /// </summary>
    /// <param name="request">The query parameters for generating the files report.</param>
    /// <param name="context">The current HTTP context associated with the request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An object containing the result of the file report, which may include success or failure details.</returns>
    public async Task<IResultDTO<FilesReportDTO[]>> HandleAsync(GenerateFilesReportQuery request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação de relatório de arquivos.");
        
        IQueryable<Document> query = dbContext
            .Documents
            .AsNoTracking()
            .Include(x => x.Client)
            .ThenInclude(c => c.Partner)
            .Include(x => x.Service)
            .Include(x => x.Responsible);
        
        if (request.PartnerId.HasValue)
            query = query.Where(d => d.Client.PartnerId == request.PartnerId.Value);

        if (request.ClientId.HasValue)
            query = query.Where(d => d.ClientId == request.ClientId.Value);

        if (request.ServiceId.HasValue)
            query = query.Where(d => d.ServiceId == request.ServiceId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(d => d.DueDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(d => d.DueDate <= request.ToDate.Value);
        
        if (request.ProgressStatus.HasValue)
            query = query.Where(d => d.ProgressStatus == request.ProgressStatus.Value);
        
        var documents = await query.ToListAsync(cancellationToken);
        
        if(documents.Count == 0)
            return ResultDTO<FilesReportDTO[]>.AsFailure(new FailureDTO(404, "Nenhum documento encontrado"));

        if (request.Status.HasValue)
            documents = documents.Where(d => d.Status == request.Status.Value).ToList();

        var result = documents.Select(x => new FilesReportDTO(x.FileName, x.Client.PartnerId,
            x.Client.Partner.LegalName, x.ClientId, x.Client.LegalName, x.ServiceId, x.Service.Name, x.ResponsibleId,
            x.Responsible.Name, x.DueDate, x.Status, x.ProgressStatus, x.Notes)).ToArray();
        
        return ResultDTO<FilesReportDTO[]>.AsSuccess(result);
    }
}