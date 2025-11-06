using Sphera.API.Clients.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.ValueObjects;
using System.Data.Entity;

namespace Sphera.API.Clients.UpdateClient;

/// <summary>
/// Handles update commands for client entities, coordinating database operations and logging within the application.
/// </summary>
/// <remarks>This handler performs update operations within a transactional scope to ensure data consistency. If
/// an error occurs or the specified client is not found, the transaction is rolled back and a failure result is
/// returned. The handler is typically used in scenarios where client information must be updated reliably and
/// auditably.</remarks>
/// <param name="dbContext">The database context used to access and update client data within the persistent store.</param>
/// <param name="logger">The logger instance used to record operational events and errors during command handling.</param>
public class UpdateClientCommandHandler(SpheraDbContext dbContext, ILogger<UpdateClientCommandHandler> logger) : IHandler<UpdateClientCommand, ClientDTO>
{
    /// <summary>
    /// Processes an update command for a client and returns the result of the operation asynchronously.
    /// </summary>
    /// <remarks>The operation is performed within a database transaction. If the client is not found or an
    /// error occurs during the update, the transaction is rolled back and a failure result is returned.</remarks>
    /// <param name="request">The update command containing the client identifier and new client information to be applied.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A result object containing the updated client data if the operation succeeds; otherwise, a failure result with
    /// error details.</returns>
    public async Task<IResultDTO<ClientDTO>> HandleAsync(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            Client? client = await dbContext.Clients.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (client is null)
                return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(400, $"Cliente não encontrado"));

            CnpjValueObject cnpj = new(request.Cnpj);
            AddressValueObject address = request.Address.ToValueObject();

            client.UpdateBasicInfo(request.TradeName, request.LegalName, cnpj, request.StateRegistration, request.MunicipalRegistration, 
                address, request.BillingDueDay, Guid.Empty);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<ClientDTO>.AsSuccess(client.ToDTO(includePartner: false));
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(500, "Erro ao atualizar cliente"));
        }
    }
}
