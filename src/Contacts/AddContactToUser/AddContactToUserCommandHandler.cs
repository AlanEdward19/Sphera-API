using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Contacts.AddContactToUser;

public class AddContactToUserCommandHandler(SpheraDbContext dbContext, ILogger<AddContactToUserCommandHandler> logger)
    : IHandler<AddContactToUserCommand, ContactDTO>
{
    public async Task<IResultDTO<ContactDTO>> HandleAsync(AddContactToUserCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adicionando contato para o Usuário: '{UserId}'.", request.GetUserId());
        
        if(await dbContext.Users.FindAsync([request.GetUserId()], cancellationToken) is null)
            return ResultDTO<ContactDTO>.AsFailure(new FailureDTO(404, "Usuário não encontrado."));

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var contact = new Contact(request.Type, request.Role, request.Value, actor, userId: request.GetUserId(), phoneType: request.PhoneType);
            await dbContext.Contacts.AddAsync(contact, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsSuccess(contact.ToDTO());
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception e)
        {
            logger.LogError($"Um erro ocorreu ao tentar adicionar um contato para o Parceiro: '{request.GetUserId()}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar adicionar o contato para o Parceiro."));
        }
    }
}