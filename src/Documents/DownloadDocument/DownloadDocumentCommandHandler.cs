using Sphera.API.Documents.UploadDocument;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.DownloadDocument;

public class DownloadDocumentCommandHandler(
    SpheraDbContext dbContext,
    ILogger<UploadDocumentCommandHandler> logger,
    IStorage storage)
    : IHandler<DownloadDocumentCommand, (Stream, string)>
{
    public async Task<IResultDTO<(Stream, string)>> HandleAsync(DownloadDocumentCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando download do documento: '{RequestId}'", request.GetId());

        var document = await dbContext.Documents.FindAsync([request.GetId()], cancellationToken);

        if (document is null)
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(404, "Documento não encontrado"));

        try
        {
            var fileName =
                $"{document.ClientId}/{document.ServiceId}/{FileNameSanitizerUtils.SanitizeName(request.GetId().ToString())}.pdf";
            
            if (await storage.ExistsAsync(fileName, cancellationToken)) 
            {
                var result = await storage.DownloadAsync(fileName, cancellationToken);
                return ResultDTO<(Stream, string)>.AsSuccess((result, document.FileName));
            }
            else 
                return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(404, "Documento não encontrado"));

            
        }
        catch (DomainException ex)
        {
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(500, "Erro ao criar documento."));
        }
    }
}