using System.Data.Entity;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Contacts.EditContact;

public class EditContactCommandHandler(SpheraDbContext dbContext, ILogger<EditContactCommandHandler> logger) : IHandler<EditContactCommand, ContactDTO>
{
    public async Task<IResultDTO<ContactDTO>> HandleAsync(EditContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Iniciando atualização do contato: '{request.GetId()}'.");

        Contact? contact = await dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == request.GetId(), cancellationToken);

        if (contact is null)
            return ResultDTO<ContactDTO>.AsFailure(
                new FailureDTO(404, "Contato não encontrado."));

        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Value))
                contact.UpdateValue(request.Value, Guid.Empty);  // TODO: substituir Guid.Empty pelo ID do usuário que está realizando a ação

            if (request.Type is not null)
                contact.UpdateType(request.Type, Guid.Empty);  // TODO: substituir Guid.Empty pelo ID do usuário que está realizando a ação

            if (request.Role is not null)
                contact.UpdateRole(request.Role, Guid.Empty);  // TODO: substituir Guid.Empty pelo ID do usuário que está realizando a ação

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