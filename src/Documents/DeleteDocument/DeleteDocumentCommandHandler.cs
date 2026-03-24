using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.DeleteDocument;

public class DeleteDocumentCommandHandler(
    SpheraDbContext dbContext,
    ILogger<DeleteDocumentCommandHandler> logger,
    [FromKeyedServices("documents")] IStorage storage) : IHandler<DeleteDocumentCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(DeleteDocumentCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando exclusão de documento {DocumentId}", request.Id);

        Document? document = await dbContext.Documents.FindAsync([request.Id], cancellationToken);

        if (document is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Documento não encontrado"));

        var fileName =
            $"{document.ClientId}/{document.ServiceId}/{FileNameSanitizerUtils.SanitizeName(request.Id.ToString())}.pdf";

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);
                dbContext.Documents.Remove(document);
                await dbContext.SaveChangesAsync(cancellationToken);

                await storage.DeleteAsync(fileName, cancellationToken);

                await dbContext.Database.CommitTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsSuccess(true);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao deletar documento."));
            }
        }, cancellationToken);
    }
}