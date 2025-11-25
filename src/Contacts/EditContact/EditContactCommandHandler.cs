using System.Data.Entity;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Contacts.EditContact;

public class EditContactCommandHandler(SpheraDbContext dbContext, ILogger<EditContactCommandHandler> logger) : IHandler<EditContactCommand, ContactDTO>
{
    public async Task<IResultDTO<ContactDTO>> HandleAsync(EditContactCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Iniciando atualização do contato: '{request.GetId()}'.");

        Contact? contact = await dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == request.GetId(), cancellationToken);

        if (contact is null)
            return ResultDTO<ContactDTO>.AsFailure(
                new FailureDTO(404, "Contato não encontrado."));

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Value))
                contact.UpdateValue(request.Value, actor);

            if (request.Type is not null)
                contact.UpdateType(request.Type, actor);

            if (request.Role is not null)
                contact.UpdateRole(request.Role, actor);
            
            if (request.PhoneType is not null)
                contact.UpdatePhoneType(request.PhoneType, actor);

            dbContext.Contacts.Update(contact);
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
            logger.LogError("Um erro ocorreu ao tentar atualizar o contato.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsFailure(
                new FailureDTO(500, "Um erro ocorreu ao tentar atualizar o contato."));
        }
    }
}