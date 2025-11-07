using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Clients.DeleteClient;

/// <summary>
/// Handles requests to delete a client from the database and provides the result of the operation.
/// </summary>
/// <remarks>The delete operation is executed within a database transaction to ensure data integrity. If the
/// specified client does not exist, the handler returns a failure result with a 404 status code. If an error occurs
/// during deletion, the transaction is rolled back and a failure result with a 500 status code is returned. This
/// handler is typically used in command-based architectures to encapsulate the logic for client deletion.</remarks>
/// <param name="dbContext">The database context used to access and modify client data.</param>
/// <param name="logger">The logger used to record informational and error messages during the delete operation.</param>
public class DeleteClientCommandHandler(SpheraDbContext dbContext, ILogger<DeleteClientCommandHandler> logger) : IHandler<DeleteClientCommand, bool>
{
	/// <summary>
	/// Processes a request to delete a client and returns the result of the operation.
	/// </summary>
	/// <remarks>The operation is performed within a database transaction. If the client does not exist, a failure
	/// result with a 404 status code is returned. If an error occurs during deletion, the transaction is rolled back and a
	/// failure result with a 500 status code is returned.</remarks>
	/// <param name="request">The command containing the client identifier to be deleted. Must not be null.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the delete operation.</param>
	/// <returns>A ResultDTO<bool> indicating whether the client was successfully deleted. Returns a failure result if the client is
	/// not found or if an error occurs during deletion.</returns>
    public async Task<IResultDTO<bool>> HandleAsync(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando exclusão de cliente {ClientId}", request.Id);

        await dbContext.Database.BeginTransactionAsync(cancellationToken);

		try
		{
			Client? client = await dbContext.Clients.FindAsync(request.Id, cancellationToken);

            if (client is null)
				return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Cliente não encontrado"));

			dbContext.Clients.Remove(client);
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
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao deletar cliente."));
        }
    }
}
