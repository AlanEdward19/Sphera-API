using Sphera.API.Clients;
using Sphera.API.Clients.DTOs;
using Sphera.API.Clients.UpdateClient;
using Sphera.API.External.Database;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.ValueObjects;
using System.Data.Entity;

namespace Sphera.API.Partners.UpdatePartner;

public class UpdatePartnerCommandHandler(SpheraDbContext dbContext, ILogger<UpdateClientCommandHandler> logger) : IHandler<UpdatePartnerCommand, PartnerDTO>
{
    public async Task<IResultDTO<PartnerDTO>> HandleAsync(UpdatePartnerCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            Partner? partner = await dbContext.Partners.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (partner is null)
                return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(400, $"Parceiro não encontrado"));

            CnpjValueObject? cnpj = string.IsNullOrWhiteSpace(request.Cnpj) ? null : new(request.Cnpj);
            AddressValueObject? address = request.Address?.ToValueObject();

            partner.UpdateBasicInfo(request.LegalName, cnpj, address, Guid.Empty);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<PartnerDTO>.AsSuccess(partner.ToDTO(includeClients:false));
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
    }
}
