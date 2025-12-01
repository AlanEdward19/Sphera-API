using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Clients.ActivateClient;

/// <summary>
/// Handles commands to activate a client by updating its status in the database.
/// </summary>
/// <remarks>The handler performs the activation operation within a database transaction to ensure data
/// consistency. If the specified client does not exist, the handler returns a failure result indicating a 404 error. If
/// an error occurs during the activation process, a failure result with a 500 error code is returned. This handler is
/// typically used in command processing pipelines to manage client activation requests.</remarks>
/// <param name="dbContext">The database context used to access and update client data.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling.</param>
public class ActivateClientCommandHandler(SpheraDbContext dbContext, ILogger<ActivateClientCommandHandler> logger)
    : IHandler<ActivateClientCommand, bool>
{
    /// <summary>
    /// Handles the activation of a client by updating its status to active.
    /// </summary>
    /// <remarks>If the specified client does not exist, the result will indicate failure with a 404 error
    /// code. If an error occurs during the activation process, the result will indicate failure with a 500 error code.
    /// The operation is performed within a database transaction to ensure data consistency.</remarks>
    /// <param name="request">The command containing the client identifier and any additional data required to activate the client.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A result object containing a boolean value that indicates whether the client was successfully activated. Returns
    /// a failure result if the client is not found or if an error occurs during the operation.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(ActivateClientCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Definindo status do Cliente: '{request.Id}' para ativado.");

        Client? client = await dbContext.Clients.FindAsync([request.Id], cancellationToken);

        if (client is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Cliente não encontrado"));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                var user = context.User;
                var actor = user.GetUserId();

                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                client.Activate(actor);
                dbContext.Clients.Update(client);

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
                logger.LogError("Um erro ocorreu ao tentar definir o status do cliente para ativo.", e);
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(500,
                    "Um erro ocorreu ao tentar definir o status do cliente para ativo."));
            }
        });

        return result;
    }
}