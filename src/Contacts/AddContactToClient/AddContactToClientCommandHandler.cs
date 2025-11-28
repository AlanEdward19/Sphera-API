using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Contacts.AddContactToClient;

/// <summary>
/// Handles the command to add a new contact to a client.
/// </summary>
/// <param name="dbContext">The database context used to access and modify client and contact data. Cannot be null.</param>
/// <param name="logger">The logger used to record informational and error messages related to the command handling process. Cannot be null.</param>
public class AddContactToClientCommandHandler(SpheraDbContext dbContext, ILogger<AddContactToClientCommand> logger) : IHandler<AddContactToClientCommand, ContactDTO>
{
    /// <summary>
    /// Adds a new contact to the specified client asynchronously.
    /// </summary>
    /// <param name="request">The command containing the details of the contact to add and the identifier of the client to which the contact
    /// will be associated. Cannot be null.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IResultDTO{ContactDTO}"/> indicating the outcome of the operation. On success, the result contains the
    /// added contact's data; on failure, it contains error information.</returns>
    public async Task<IResultDTO<ContactDTO>> HandleAsync(AddContactToClientCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Adicionando contato para o Cliente: '{request.GetClientId()}'.");

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            Contact contact = new Contact(request.Type, request.Role, request.Value, createdBy: actor, userId: actor,
                clientId: request.GetClientId(), name: request.Name);

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
            logger.LogError($"Um erro ocorreu ao tentar adicionar um contato para o Cliente: '{request.GetClientId()}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar adicionar o contato para o Cliente."));
        }
    }
}