using Microsoft.EntityFrameworkCore;
using Sphera.API.Documents.DTOs;
using Sphera.API.Documents.GetDocuments;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.GetDocumentById;

public class GetDocumentByIdQueryHandler(SpheraDbContext dbContext,
    ILogger<GetDocumentsQueryHandler> logger,
    IStorage storage) : IHandler<GetDocumentByIdQuery, DocumentWithMetadataDTO>
{
    public async Task<IResultDTO<DocumentWithMetadataDTO>> HandleAsync(GetDocumentByIdQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando recuperação do documento pelo ID: '{DocumentId}'", request.Id);

        IQueryable<Document> query = dbContext
            .Documents
            .AsNoTracking()
            .Include(x => x.Client)
            .ThenInclude(c => c.Partner)
            .Include(x => x.Service)
            .Include(x => x.Responsible);
        
        var document = await query.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        
        if (document is null)
            return ResultDTO<DocumentWithMetadataDTO>.AsFailure(new FailureDTO(404, "Documento não encontrado"));
        
        var fileName =
            $"{document.ClientId}/{document.ServiceId}/{FileNameSanitizerUtils.SanitizeName(request.Id.ToString())}.pdf";
        
        var metadata = await storage.GetBlobClientWithSasAsync(fileName, null, cancellationToken);

        
        
        if (metadata is null)
            return ResultDTO<DocumentWithMetadataDTO>.AsSuccess(document.ToDTO(null));
        
        var fileMetadata = new FileMetadataDTO(
            metadata!.Value.blobClient.Name,
            (await metadata.Value.blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value
            .ContentLength,
            (await metadata.Value.blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value
            .ContentType,
            metadata.Value.sasUri.ToString()
        );
        
        var documentDto = document.ToDTO(fileMetadata);
        
        return ResultDTO<DocumentWithMetadataDTO>.AsSuccess(documentDto);
    }
}