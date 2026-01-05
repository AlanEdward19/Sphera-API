using Microsoft.EntityFrameworkCore;
using Sphera.API.Documents.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.GetDocuments;

public class GetDocumentsQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetDocumentsQueryHandler> logger,
    IStorage storage) : IHandler<GetDocumentsQuery, IEnumerable<DocumentWithMetadataDTO>>
{
    public async Task<IResultDTO<IEnumerable<DocumentWithMetadataDTO>>> HandleAsync(GetDocumentsQuery request,
        HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando recuperação de documentos para o cliente: '{ClientId}'", request.ClientId);

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

        if (request.DueDateFrom.HasValue)
            query = query.Where(d => d.DueDate >= request.DueDateFrom.Value);

        if (request.DueDateTo.HasValue)
            query = query.Where(d => d.DueDate <= request.DueDateTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchLower = request.Search!.ToLower();
            query = query.Where(d => EF.Functions.Like((d.FileName ?? string.Empty).ToLower(), $"%{searchLower}%")
                                     || EF.Functions.Like((d.Client.LegalName ?? string.Empty).ToLower(),
                                         $"%{searchLower}%")
                                     || EF.Functions.Like((d.Client.TradeName ?? string.Empty).ToLower(),
                                         $"%{searchLower}%")
                                     || EF.Functions.Like((d.Responsible.Name ?? string.Empty).ToLower(),
                                         $"%{searchLower}%")
                                     || EF.Functions.Like((d.Client.Partner.LegalName ?? string.Empty).ToLower(),
                                         $"%{searchLower}%")
                                     || EF.Functions.Like(d.Service.Name.ToLower() ?? string.Empty, $"%{searchLower}%")
            );
        }

        var documents = await query
            .Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : 0))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        if (request.Status.HasValue)
            documents = documents.Where(d => d.Status == request.Status.Value).ToList();

        List<DocumentWithMetadataDTO> result = [];

        foreach (var document in documents)
        {
            var fileName =
                $"{document.ClientId}/{document.ServiceId}/{FileNameSanitizerUtils.SanitizeName(document.Id.ToString())}.pdf";

            var metadata = await storage.GetBlobClientWithSasAsync(fileName, null, cancellationToken);
            
            if (metadata is null)
            {
                result.Add(document.ToDTO(null));
                continue;
            }
            
            var fileMetadata = new FileMetadataDTO(
                metadata!.Value.blobClient.Name,
                (await metadata.Value.blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value
                .ContentLength,
                (await metadata.Value.blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value
                .ContentType,
                metadata.Value.sasUri.ToString()
            );
            
            result.Add(document.ToDTO(fileMetadata));
        }

        return ResultDTO<IEnumerable<DocumentWithMetadataDTO>>.AsSuccess(result);
    }
}