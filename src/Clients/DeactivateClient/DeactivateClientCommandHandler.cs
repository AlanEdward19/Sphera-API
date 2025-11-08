using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Clients.DeactivateClient;

/// <summary>
/// Handles commands to deactivate a client in the system.
/// </summary>
/// <remarks>The operation is performed within a database transaction to ensure data consistency. If the specified
/// client does not exist, the handler returns a failure result with a 404 error code. If an error occurs during the
/// operation, a failure result with a 500 error code is returned.</remarks>
/// <param name="dbContext">The database context used to access and update client data.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling.</param>
public class DeactivateClientCommandHandler(SpheraDbContext dbContext, ILogger<DeactivateClientCommandHandler> logger) : IHandler<DeactivateClientCommand, bool>
{
    /// <summary>
    /// Handles the deactivation of a client based on the specified command.
    /// </summary>
    /// <remarks>If the specified client does not exist, the result will indicate failure with a 404 error
    /// code. If an error occurs during the operation, the result will indicate failure with a 500 error code. The
    /// operation is performed within a database transaction to ensure consistency.</remarks>
    /// <param name="request">The command containing the client identifier and any additional information required to perform the
    /// deactivation.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns>A result object indicating whether the client was successfully deactivated. Returns a failure result if the
    /// client is not found or if an error occurs during the operation.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(DeactivateClientCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Definindo status do Cliente: '{request.Id}' para desativado.");

        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            Client? client = await dbContext.Clients.FindAsync([request.Id], cancellationToken);

            if (client is null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Cliente não encontrado"));

            client.Deactivate(actor);
            dbContext.Clients.Update(client);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (Exception e)
        {
            logger.LogError("Um erro ocorreu ao tentar definir o status do cliente para desativado.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar definir o status do cliente para desativado."));
        }
    }
}