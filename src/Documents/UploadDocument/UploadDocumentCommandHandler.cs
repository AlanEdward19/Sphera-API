using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.UploadDocument;

public class UploadDocumentCommandHandler(
    SpheraDbContext dbContext,
    ILogger<UploadDocumentCommandHandler> logger,
    [FromKeyedServices("documents")] IStorage storage)
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
            var fileName =
                $"{document.ClientId}/{document.ServiceId}/{FileNameSanitizerUtils.SanitizeName(request.GetId().ToString())}.pdf";

            if (await storage.ExistsAsync(fileName, cancellationToken))
                await storage.DeleteAsync(fileName, cancellationToken);

            await storage.UploadAsync(request.GetData(), fileName, request.GetContentType(), cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException ex)
        {
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao criar documento."));
        }
    }
}