using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Services.DeactivateService;

/// <summary>
/// Handles commands to deactivate a service, coordinating database operations and logging within the application
/// context.
/// </summary>
/// <remarks>The handler performs the deactivation operation within a database transaction to ensure data
/// consistency. It returns failure results with appropriate status codes if the service is not found, a domain-specific
/// error occurs, or an unexpected error is encountered. The current user is determined from the provided HTTP context
/// to record the actor performing the operation.</remarks>
/// <param name="dbContext">The database context used to access and update service entities.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling.</param>
public class DeactivateServiceCommandHandler(SpheraDbContext dbContext, ILogger<DeactivateServiceCommandHandler> logger) : IHandler<DeactivateServiceCommand, bool>
{
    /// <summary>
    /// Handles the deactivation of a service based on the specified command and user context.
    /// </summary>
    /// <remarks>If the specified service does not exist, the result will indicate failure with a 404 status
    /// code. If a domain-specific error occurs during deactivation, the result will indicate failure with a 400 status
    /// code. Unexpected errors result in a failure with a 500 status code. The operation is performed within a database
    /// transaction to ensure consistency.</remarks>
    /// <param name="request">The command containing the identifier of the service to deactivate.</param>
    /// <param name="context">The HTTP context representing the current user and request information. Used to determine the actor performing
    /// the operation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A result object indicating whether the service was successfully deactivated. Returns a failure result if the
    /// service is not found or if an error occurs during deactivation.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(DeactivateServiceCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Desativando serviço: '{request.Id}'.");

        Service? service = await dbContext.Services.FindAsync([request.Id], cancellationToken);

        if (service is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Serviço não encontrado"));

        Guid actor = context.User.GetUserId();

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                service.Deactivate(actor);

                dbContext.Services.Update(service);
                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<bool>.AsSuccess(true);
            }
            catch (DomainException e)
            {
                logger.LogError("Um erro ocorreu ao tentar desativar o serviço", e);
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);

                return ResultDTO<bool>.AsFailure(new FailureDTO(400, e.Message));
            }
            catch (Exception e)
            {
                logger.LogError("Um erro ocorreu ao tentar desativar o serviço", e);
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);

                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro inesperado ocorreu ao tentar desativar o serviço"));
            }
        }, cancellationToken);
    }
}