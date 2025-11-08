using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Partners.ActivatePartner;

/// <summary>
/// Handles the activation of a partner by updating its status in the database.
/// </summary>
/// <param name="dbContext">The database context used to access and update partner data.</param>
/// <param name="logger">The logger used to record informational and error messages during the activation process.</param>
public class ActivatePartnerCommandHandler(SpheraDbContext dbContext, ILogger<ActivatePartnerCommandHandler> logger) : IHandler<ActivatePartnerCommand, bool>
{
    /// <summary>
    /// Activates the specified partner and updates its status in the database asynchronously.
    /// </summary>
    /// <param name="request">The command containing the identifier of the partner to activate.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result object indicating whether the partner was successfully activated. Returns a failure result if the
    /// partner is not found or if an error occurs during the operation.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(ActivatePartnerCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Definindo status do Parceiro: '{request.Id}' para ativado.");

        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = context.User;
            var actor = user.GetUserId();
            
            Partner? partner = await dbContext.Partners.FindAsync([request.Id], cancellationToken);

            if (partner is null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Parceiro não encontrado"));

            partner.Activate(actor);
            dbContext.Partners.Update(partner);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (Exception e)
        {
            logger.LogError("Um erro ocorreu ao tentar definir o status do parceiro para ativo.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar definir o status do parceiro para ativo."));
        }
    }
}