using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.BillingEntries.GetBillingEntryById;

public class GetBillingEntryByIdQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetBillingEntryByIdQueryHandler> logger)
    : IHandler<GetBillingEntryByIdQuery, BillingEntryDTO>
{
    public async Task<IResultDTO<BillingEntryDTO>> HandleAsync(
        GetBillingEntryByIdQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando BillingEntry {Id}", request.Id);

        var entity = await dbContext.BillingEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<BillingEntryDTO>.AsFailure(
                new FailureDTO(404, "Lançamento não encontrado."));

        var dto = new BillingEntryDTO(
            entity.Id,
            entity.ClientId,
            entity.ServiceId,
            entity.ServiceDate,
            entity.Quantity,
            entity.IsBillable,
            entity.Notes,
            entity.InvoiceId);

        return ResultDTO<BillingEntryDTO>.AsSuccess(dto);
    }
}