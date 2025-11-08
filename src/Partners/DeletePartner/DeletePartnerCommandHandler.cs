using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Partners.DeletePartner;

/// <summary>
/// Handles requests to delete a partner by processing the specified command and coordinating the deletion operation
/// within the database.
/// </summary>
/// <remarks>This handler performs the delete operation within a database transaction to ensure data consistency.
/// If the specified partner does not exist, the operation results in a failure with a 404 error code. Any errors during
/// deletion result in a failure with a 500 error code. This class is typically used in command-based architectures to
/// encapsulate the logic for deleting partners.</remarks>
/// <param name="dbContext">The database context used to access and modify partner data. Cannot be null.</param>
/// <param name="logger">The logger used to record informational and error messages during the delete operation. Cannot be null.</param>
public class DeletePartnerCommandHandler(SpheraDbContext dbContext, ILogger<DeletePartnerCommandHandler> logger) : IHandler<DeletePartnerCommand, bool>
{
    /// <summary>
    /// Processes a request to delete a partner asynchronously and returns the result of the operation.
    /// </summary>
    /// <remarks>If the specified partner does not exist, the method returns a failure result with a 404 error
    /// code. If an error occurs during the deletion process, a failure result with a 500 error code is returned. The
    /// operation is performed within a database transaction to ensure consistency.</remarks>
    /// <param name="request">The command containing the identifier of the partner to be deleted. Cannot be null.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A token that can be used to cancel the delete operation.</param>
    /// <returns>A ResultDTO<bool> indicating whether the partner was successfully deleted. Returns a failure result if the
    /// partner is not found or if an error occurs during deletion.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(DeletePartnerCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando exclusão de parceiro {PartnerId}", request.Id);

        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            Partner? partner = await dbContext.Partners.FindAsync(request.Id, cancellationToken);

            if (partner is null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Parceiro não encontrado"));

            dbContext.Partners.Remove(partner);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao deletar parceiro."));
        }
    }
}
