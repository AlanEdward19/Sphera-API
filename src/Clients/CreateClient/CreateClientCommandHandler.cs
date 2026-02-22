using Microsoft.EntityFrameworkCore;
using Sphera.API.Clients.DTOs;
using Sphera.API.Contacts.Enums;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Clients.CreateClient;

/// <summary>
/// Handles the creation of a new client entity in the database based on the provided command.
/// </summary>
/// <remarks>This handler performs transactional operations to ensure that client creation is atomic. If the
/// specified partner does not exist, the operation fails with a not found result. All changes are committed only if the
/// client is successfully created; otherwise, the transaction is rolled back. Logging is performed to aid in monitoring
/// and troubleshooting.</remarks>
/// <param name="dbContext">The database context used to access and persist client and partner data.</param>
/// <param name="logger">The logger instance used to record informational and error messages during client creation.</param>
public class CreateClientCommandHandler(SpheraDbContext dbContext, ILogger<CreateClientCommandHandler> logger)
    : IHandler<CreateClientCommand, ClientDTO>
{
    /// <summary>
    /// Processes a request to create a new client associated with a specified partner.
    /// </summary>
    /// <remarks>If the specified partner does not exist, the method returns a failure result with a 404 error
    /// code. In case of an unexpected error during client creation, a failure result with a 500 error code is returned.
    /// The operation is performed within a database transaction to ensure consistency.</remarks>
    /// <param name="request">The command containing the details required to create the client, including the partner identifier and client
    /// information.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A result object containing the created client data if successful; otherwise, a failure result with error
    /// details.</returns>
    public async Task<IResultDTO<ClientDTO>> HandleAsync(CreateClientCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação de cliente para o parceiro {PartnerId}", request.PartnerId);

        if (await dbContext.Partners.FindAsync([request.PartnerId], cancellationToken) is null)
            return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(400, "Parceiro não encontrado."));
        
        var normalizedCnpj = new CnpjValueObject(request.Cnpj);
        var cnpjExists = await dbContext.Clients.AnyAsync(c => c.Cnpj == normalizedCnpj, cancellationToken);
        if (cnpjExists)
            return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(400, "CNPJ já cadastrado."));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                var user = context.User;
                var actor = user.GetUserId();

                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                Client client = new(request, actor);
                
                client.AddContact(EContactType.Email, EContactRole.Financial, request.FinancialEmail, actor, request.FinancialContactName);
                client.AddContact(EContactType.Email, EContactRole.Personal, request.Email, actor, request.ContactName);
                client.AddContact(EContactType.Phone, EContactRole.Financial, request.FinancialPhone, actor, request.FinancialContactName);
                client.AddContact(EContactType.Phone, EContactRole.Personal, request.Phone, actor, request.ContactName);
                
                await dbContext.AddAsync(client, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<ClientDTO>.AsSuccess(client.ToDTO(includePartner: false, 0));
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception e)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(500, "Erro ao criar cliente."));
            }
        });
        
        return result;
    }
}