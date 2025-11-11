using Sphera.API.Documents.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Documents.UploadDocument;

public class UploadDocumentCommandHandler(SpheraDbContext dbContext, ILogger<UploadDocumentCommandHandler> logger, IStorage storage)
    : IHandler<UploadDocumentCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(UploadDocumentCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando upload do documento: '{RequestId}'", request.GetId());
        
        var document = await dbContext.Documents.FindAsync([request.GetId()], cancellationToken);

        if (document is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Documento não encontrado"));

        try
        {
            var user = context.User;
            var actor = user.GetUserId();

            await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            var fileName = $"{document.ClientId}/{document.ServiceId}/{request.GetId()}/{request.FileName}.pdf";
            
            var fileUrl = await storage.UploadAsync(request.GetData(), fileName, request.GetContentType(), cancellationToken);
            
            FileMetadataValueObject fileMetadata = new(request.FileName, request.GetSize(),request.GetContentType(), fileUrl);
            document.UpdateFile(fileMetadata, actor);
            dbContext.Documents.Update(document);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception e)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao criar documento."));
        }
    }
}