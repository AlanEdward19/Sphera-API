using Microsoft.EntityFrameworkCore;
using Sphera.API.Documents;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Reports.GenerateFilesReport;

public class GenerateFilesReportQueryHandler(SpheraDbContext dbContext, ILogger<GenerateFilesReportQueryHandler> logger) : IHandler<GenerateFilesReportQuery, FilesReportDTO[]>
{
    public async Task<IResultDTO<FilesReportDTO[]>> HandleAsync(GenerateFilesReportQuery request, HttpContext context, CancellationToken cancellationToken)
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
        
        var documents = await query.ToListAsync(cancellationToken);
        
        if(!documents.Any())
            return ResultDTO<FilesReportDTO[]>.AsFailure(new FailureDTO(404, "Nenhum documento encontrado"));

        var result = documents.Select(x => new FilesReportDTO(x.FileName, x.Client.PartnerId,
            x.Client.Partner.LegalName, x.ClientId, x.Client.LegalName, x.ServiceId, x.Service.Name, x.ResponsibleId,
            x.Responsible.Name, x.DueDate, x.Status)).ToArray();
        
        return ResultDTO<FilesReportDTO[]>.AsSuccess(result);
    }
}