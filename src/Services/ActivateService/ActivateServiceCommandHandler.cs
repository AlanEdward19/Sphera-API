using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Services.ActivateService;

/// <summary>
/// Handles commands to activate a service by updating its state in the database.
/// </summary>
/// <remarks>This handler processes activation requests for services, ensuring that the service exists and
/// updating its state accordingly. It manages transaction boundaries and logs relevant events. If the service does not
/// exist or a domain-specific error occurs, the handler returns failure details. This class is typically used within a
/// command handling pipeline to encapsulate the activation logic for services.</remarks>
/// <param name="dbContext">The database context used to access and update service entities.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling.</param>
public class ActivateServiceCommandHandler(SpheraDbContext dbContext, ILogger<ActivateServiceCommandHandler> logger) : IHandler<ActivateServiceCommand, bool>
{
    /// <summary>
    /// Attempts to activate the specified service asynchronously and returns the result of the operation.
    /// </summary>
    /// <remarks>If the specified service does not exist, the result indicates failure with a 404 status code.
    /// If a domain-specific error occurs during activation, the result includes a 400 status code and the error
    /// message. For unexpected errors, the result includes a 500 status code and a generic error message.</remarks>
    /// <param name="request">The command containing the identifier of the service to activate.</param>
    /// <param name="context">The HTTP context associated with the current request. Used to determine the user performing the operation.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A result object containing a boolean value that is <see langword="true"/> if the service was successfully
    /// activated; otherwise, <see langword="false"/>. If the service is not found or an error occurs, the result
    /// includes failure details.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(ActivateServiceCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Ativando serviço: '{request.Id}'.");

        Service? service = await dbContext.Services.FindAsync([request.Id], cancellationToken);

        if (service is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Serviço não encontrado"));

        Guid actor = context.User.GetUserId();

        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            service.Activate(actor);

            dbContext.Services.Update(service);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException e)
        {
            logger.LogError("Um erro ocorreu ao tentar ativar o serviço", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsFailure(new FailureDTO(400, e.Message));
        }
        catch (Exception e)
        {
            logger.LogError("Um erro ocorreu ao tentar ativar o serviço", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro inesperado ocorreu ao tentar ativar o serviço"));
        }
    }
}