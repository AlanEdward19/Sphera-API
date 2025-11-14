using Sphera.API.Documents.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Documents.CreateDocument;

public class CreateDocumentCommandHandler(SpheraDbContext dbContext, ILogger<CreateDocumentCommandHandler> logger)
    : IHandler<CreateDocumentCommand, DocumentDTO>
{
    public async Task<IResultDTO<DocumentDTO>> HandleAsync(CreateDocumentCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação do documento para o cliente {ClientId}.", request.ClientId);

        if (await dbContext.Clients.FindAsync([request.ClientId], cancellationToken) is null)
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(400, "Cliente não encontrado."));
           
        if(await dbContext.Services.FindAsync([request.ServiceId], cancellationToken) is null)
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(400, "Serviço não encontrado."));
        
        if(await dbContext.Users.FindAsync([request.ResponsibleId], cancellationToken) is null)
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(400, "Usuário não encontrado."));

        try
        {
            var user = context.User;
            var actor = user.GetUserId();

            await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            Document document = new(request, actor);
            await dbContext.AddAsync(document, cancellationToken);
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
            return ResultDTO<DocumentDTO>.AsFailure(new FailureDTO(500, "Erro ao criar documento."));
        }
    }
}