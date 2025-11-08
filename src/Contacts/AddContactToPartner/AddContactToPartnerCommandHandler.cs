using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Contacts.AddContactToPartner;

/// <summary>
/// Handles the command to add a new contact to a specified partner.
/// </summary>
/// <remarks>This handler coordinates the creation of a new contact and associates it with an existing partner. It
/// manages the database transaction and logs relevant events and errors during the operation.</remarks>
/// <param name="dbContext">The database context used to access and modify partner and contact data. Cannot be null.</param>
/// <param name="logger">The logger used to record informational and error messages related to the command handling process. Cannot be null.</param>
public class AddContactToPartnerCommandHandler(SpheraDbContext dbContext, ILogger<AddContactToPartnerCommand> logger) : IHandler<AddContactToPartnerCommand, ContactDTO>
{
    /// <summary>
    /// Adds a new contact to the specified partner asynchronously.
    /// </summary>
    /// <param name="request">The command containing the details of the contact to add and the identifier of the partner to associate with the
    /// contact. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result object containing the added contact data if the operation succeeds; otherwise, a failure result with
    /// error information.</returns>
    public async Task<IResultDTO<ContactDTO>> HandleAsync(AddContactToPartnerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Adicionando contato para o Parceiro: '{request.GetPartnerId()}'.");

        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            Contact contact = new Contact(request.Type, request.Role, request.Value, Guid.Empty, request.GetPartnerId()); // TODO: substituir Guid.Empty pelo ID do usuário que está realizando a ação

            await dbContext.Contacts.AddAsync(contact, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<ContactDTO>.AsSuccess(contact.ToDTO());

        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception e)
        {
            logger.LogError($"Um erro ocorreu ao tentar adicionar um contato para o Parceiro: '{request.GetPartnerId()}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ContactDTO>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar adicionar o contato para o Parceiro."));
        }
    }
}