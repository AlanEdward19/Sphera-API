using Microsoft.EntityFrameworkCore;
using Sphera.API.Clients.UpdateClient;
using Sphera.API.External.Database;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

using Sphera.API.Shared.Utils;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Partners.UpdatePartner;

public class UpdatePartnerCommandHandler(SpheraDbContext dbContext, ILogger<UpdateClientCommandHandler> logger)
    : IHandler<UpdatePartnerCommand, PartnerDTO>
{
    public async Task<IResultDTO<PartnerDTO>> HandleAsync(UpdatePartnerCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Iniciando a atualização do parceiro: '{request.GetId()}'.");

        var partner = await dbContext.Partners.Include(x => x.Contacts)
            .FirstOrDefaultAsync(x => x.Id == request.GetId(), cancellationToken);

        if (partner is null)
            return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(400, $"Parceiro não encontrado"));

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                var user = context.User;
                var actor = user.GetUserId();

                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                CnpjValueObject? cnpj = string.IsNullOrWhiteSpace(request.Cnpj) ? null : new(request.Cnpj);
                AddressValueObject? address = request.Address?.ToValueObject();

                partner.UpdateBasicInfo(request.LegalName, cnpj, address, actor);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<PartnerDTO>.AsSuccess(partner.ToDTO(includeClients: false));
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(500, "Erro ao atualizar parceiro"));
            }
        }, cancellationToken);
    }
}