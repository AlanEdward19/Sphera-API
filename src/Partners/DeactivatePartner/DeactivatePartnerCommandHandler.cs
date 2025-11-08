using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Partners.DeactivatePartner;

/// <summary>
/// Handles commands to deactivate a partner in the system.
/// </summary>
/// <remarks>The handler performs the deactivation operation within a database transaction to ensure data
/// consistency. If the specified partner does not exist, the operation will fail with a 404 error code. Unexpected
/// errors result in a failure with a 500 error code.</remarks>
/// <param name="dbContext">The database context used to access and update partner data.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling.</param>
public class DeactivatePartnerCommandHandler(SpheraDbContext dbContext, ILogger<DeactivatePartnerCommand> logger) : IHandler<DeactivatePartnerCommand, bool>
{
    /// <summary>
    /// Handles the deactivation of a partner based on the specified command.
    /// </summary>
    /// <remarks>If the specified partner does not exist, the result will indicate failure with a 404 error
    /// code. In case of an unexpected error, the result will indicate failure with a 500 error code. The operation is
    /// performed within a database transaction to ensure consistency.</remarks>
    /// <param name="request">The command containing the information required to identify and deactivate the partner.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
    /// <returns>A result object indicating whether the partner was successfully deactivated. Returns a failure result if the
    /// partner is not found or if an error occurs during the operation.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(DeactivatePartnerCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Definindo status do Parceiro: '{request.Id}' para desativado.");
        
        Partner? partner = await dbContext.Partners.FindAsync([request.Id], cancellationToken);

        if (partner is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Parceiro não encontrado"));

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            partner.Deactivate(actor);
            dbContext.Partners.Update(partner);

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
            logger.LogError("Um erro ocorreu ao tentar definir o status do parceiro para desativado.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar definir o status do parceiro para desativado."));
        }
    }
}