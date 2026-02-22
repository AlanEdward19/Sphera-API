using Microsoft.EntityFrameworkCore;
using Sphera.API.Contacts.Enums;
using Sphera.API.External.Database;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;
using Sphera.API.Shared.Utils;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Partners.CreatePartner;

/// <summary>
/// Handles the creation of a new partner entity in the database using the specified command.
/// </summary>
/// <remarks>This handler ensures that partner creation is performed within a transactional scope for data
/// consistency. Logging is performed for auditing and troubleshooting purposes. The handler is typically invoked by
/// application infrastructure to process partner creation requests.</remarks>
/// <param name="dbContext">The database context used to access and persist partner data.</param>
/// <param name="logger">The logger used to record informational and error messages during partner creation.</param>
public class CreatePartnerCommandHandler(SpheraDbContext dbContext, ILogger<CreatePartnerCommandHandler> logger) : IHandler<CreatePartnerCommand, PartnerDTO>
{
    /// <summary>
    /// Asynchronously creates a new partner entity based on the specified command and returns the result.
    /// </summary>
    /// <remarks>This method initiates a database transaction to ensure atomicity of the partner creation
    /// process. If an error occurs, the transaction is rolled back and a failure result is returned. The operation is
    /// logged for auditing purposes.</remarks>
    /// <param name="request">The command containing the data required to create a new partner.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A result object containing the created partner data if successful; otherwise, a failure result with error
    /// details.</returns>
    public async Task<IResultDTO<PartnerDTO>> HandleAsync(CreatePartnerCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação de parceiro");
        
        if (!string.IsNullOrWhiteSpace(request.Cnpj))
        {
            var normalizedCnpj = new CnpjValueObject(request.Cnpj);
            var cnpjExists = await dbContext.Partners.AnyAsync(p => p.Cnpj != null && p.Cnpj == normalizedCnpj, cancellationToken);
            if (cnpjExists)
                return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(400, "CNPJ já cadastrado."));
        }

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                var user = context.User;
                var actor = user.GetUserId();
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                Partner partner = new(request, actor);

                if (!string.IsNullOrWhiteSpace(request.FinancialEmail))
                    partner.AddContact(EContactType.Email, EContactRole.Financial, request.FinancialEmail, actor);
                
                if (!string.IsNullOrWhiteSpace(request.FinancialPhone))
                    partner.AddContact(EContactType.Phone, EContactRole.Financial, request.FinancialPhone, actor);
                
                if (!string.IsNullOrWhiteSpace(request.ResponsibleEmail))
                    partner.AddContact(EContactType.Email, EContactRole.Personal, request.ResponsibleEmail, actor);
                
                if (!string.IsNullOrWhiteSpace(request.ResponsiblePhone))
                    partner.AddContact(EContactType.Phone, EContactRole.Personal, request.ResponsiblePhone, actor);

                if (!string.IsNullOrWhiteSpace(request.LandLine))
                    partner.AddContact(EContactType.Phone, EContactRole.General, request.LandLine, actor,
                        EPhoneType.Landline);

                if (!string.IsNullOrWhiteSpace(request.BackupPhone))
                    partner.AddContact(EContactType.Phone, EContactRole.General, request.BackupPhone, actor,
                        EPhoneType.Backup);
                
                partner.AddContact(EContactType.Phone, EContactRole.General, request.Phone, actor, EPhoneType.Mobile);

                await dbContext.AddAsync(partner, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<PartnerDTO>.AsSuccess(partner.ToDTO(includeClients: false, 0));
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(500, "Erro ao criar parceiro."));
            }
        }, cancellationToken);
    }
}