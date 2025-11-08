using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Contacts.RemoveContactFromClient;

/// <summary>
/// Handles the command to remove a contact from a client in the database.
/// </summary>
/// <remarks>This handler performs the removal operation within a database transaction to ensure data consistency.
/// If the specified contact does not exist for the given client, the handler returns a failure result with a 404 error
/// code. In the event of an unexpected error, a failure result with a 500 error code is returned. The handler is
/// typically used in application service layers to process contact removal requests.</remarks>
/// <param name="dbContext">The database context used to access and modify client and contact data. Cannot be null.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling. Cannot be null.</param>
public class RemoveContactFromClientCommandHandler(SpheraDbContext dbContext, ILogger<RemoveContactFromClientCommandHandler> logger) : IHandler<RemoveContactFromClientCommand, bool>
{
    /// <summary>
    /// Asynchronously removes a contact from a client based on the specified command.
    /// </summary>
    /// <remarks>If the contact does not exist for the specified client, the operation returns a failure
    /// result with a 404 error. In case of an unexpected error, the operation returns a failure result with a 500 error
    /// code. The operation is performed within a database transaction.</remarks>
    /// <param name="request">The command containing the identifiers of the client and contact to be removed. Cannot be null.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing a boolean value that is <see langword="true"/> if the contact was successfully
    /// removed; otherwise, <see langword="false"/>. If the contact is not found, the result indicates failure with a
    /// 404 error code.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(RemoveContactFromClientCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Removendo contato para o Cliente: '{request.ClientId}'.");

        Contact? contact =
            await dbContext.Contacts.FirstOrDefaultAsync(
                x => x.Id == request.ContactId && x.ClientId == request.ClientId, cancellationToken);

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
            logger.LogError($"Um erro ocorreu ao tentar remover o contato: '{request.ContactId}' para o Cliente: '{request.ClientId}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar remover o contato para o Cliente."));
        }
    }
}