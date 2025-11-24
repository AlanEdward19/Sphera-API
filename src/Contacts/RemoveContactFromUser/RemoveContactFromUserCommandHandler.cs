using Sphera.API.Contacts.RemoveContactFromPartner;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Contacts.RemoveContactFromUser;

public class RemoveContactFromUserCommandHandler(SpheraDbContext dbContext, ILogger<RemoveContactFromPartnerCommandHandler> logger) 
    : IHandler<RemoveContactFromUserCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(RemoveContactFromUserCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removendo contato para o Usuário: '{UserId}'.", request.UserId);
        
        Contact ? contact =
            dbContext.Contacts.FirstOrDefault(
                x => x.Id == request.ContactId && x.UserId == request.UserId);
        if (contact is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Contato não encontrado"));

        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);
            dbContext.Contacts.Remove(contact);
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
            logger.LogError($"Um erro ocorreu ao tentar remover o contato: '{request.ContactId}' para o Usuário: '{request.UserId}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar remover o contato para o Usuário."));
        }
    }
}