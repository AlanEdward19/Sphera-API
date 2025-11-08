using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Contacts.RemoveContactFromPartner;

/// <summary>
/// Handles the command to remove a contact from a partner in the system.
/// </summary>
/// <remarks>This handler coordinates the removal of a contact associated with a specific partner, ensuring
/// transactional integrity. It logs relevant information and errors during the operation. The handler returns a result
/// indicating success or failure, including appropriate error information if the contact is not found or if an error
/// occurs during the process.</remarks>
/// <param name="dbContext">The database context used to access and modify partner and contact data.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling.</param>
public class RemoveContactFromPartnerCommandHandler(SpheraDbContext dbContext, ILogger<RemoveContactFromPartnerCommandHandler> logger) : IHandler<RemoveContactFromPartnerCommand, bool>
{
    /// <summary>
    /// Handles the removal of a contact from a partner asynchronously.
    /// </summary>
    /// <remarks>If the contact does not exist for the specified partner, the operation returns a failure
    /// result with a 404 error. In case of an unexpected error, a failure result with a 500 error code is returned. The
    /// operation is performed within a database transaction to ensure consistency.</remarks>
    /// <param name="request">The command containing the identifiers of the partner and the contact to be removed.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result object containing a boolean value that is <see langword="true"/> if the contact was successfully
    /// removed; otherwise, <see langword="false"/>. If the contact is not found, the result indicates failure with a
    /// 404 error code.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(RemoveContactFromPartnerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Removendo contato para o Parceiro: '{request.PartnerId}'.");

        Contact? contact =
            await dbContext.Contacts.FirstOrDefaultAsync(
                x => x.Id == request.ContactId && x.PartnerId == request.PartnerId, cancellationToken);

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
            logger.LogError($"Um erro ocorreu ao tentar remover o contato: '{request.ContactId}' para o Parceiro: '{request.PartnerId}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar remover o contato para o Parceiro."));
        }
    }
}