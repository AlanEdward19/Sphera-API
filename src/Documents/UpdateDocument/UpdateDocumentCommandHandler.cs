using Sphera.API.Documents.DeleteDocument;
using Sphera.API.Documents.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.UpdateDocument;

public class UpdateDocumentCommandHandler(SpheraDbContext dbContext,
    ILogger<DeleteDocumentCommandHandler> logger) : IHandler<UpdateDocumentCommand, DocumentDTO>
{
    public async Task<IResultDTO<DocumentDTO>> HandleAsync(UpdateDocumentCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando atualização do documento {DocumentId}", request.GetId());
        
        Document? document = await dbContext.Documents.FindAsync([request.GetId()], cancellationToken);

        if (document is null)
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(404, "Documento não encontrado"));

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            document.ChangeClient(request.ClientId, actor);
            document.ChangeService(request.ServiceId, actor);
            document.ChangeResponsible(request.ResponsibleId, actor);
            document.UpdateDates(request.IssueDate, request.DueDate, actor);
            document.AddNotes(request.Notes, actor);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);
            
            return ResultDTO<DocumentDTO>.AsSuccess(document.ToDTO());
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(500, "Erro ao atualizar o documento."));
        }
    }
}