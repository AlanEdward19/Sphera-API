using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.BillingEntries.ListBillingEntries;

public class ListBillingEntriesQueryHandler(
    SpheraDbContext dbContext,
    ILogger<ListBillingEntriesQueryHandler> logger)
    : IHandler<ListBillingEntriesQuery, IReadOnlyCollection<BillingEntryDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<BillingEntryDTO>>> HandleAsync(
        ListBillingEntriesQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listando BillingEntries");

        IQueryable<BillingEntry> query = dbContext.BillingEntries.AsNoTracking();

        if (request.ClientId.HasValue)
            query = query.Where(x => x.ClientId == request.ClientId.Value);

        if (request.ServiceDateStart.HasValue)
            query = query.Where(x => x.ServiceDate >= request.ServiceDateStart.Value.Date);

        if (request.ServiceDateEnd.HasValue)
            query = query.Where(x => x.ServiceDate <= request.ServiceDateEnd.Value.Date);

        if (request.IsBillable.HasValue)
            query = query.Where(x => x.IsBillable == request.IsBillable.Value);

        if (request.OnlyWithoutInvoice)
            query = query.Where(x => x.InvoiceId == null);

        var list = await query
            .OrderBy(x => x.ClientId)
            .ThenBy(x => x.ServiceDate)
            .ToListAsync(cancellationToken);

        var dtos = list
            .Select(x => new BillingEntryDTO(
                x.Id,
                x.ClientId,
                x.ServiceId,
                x.ServiceDate,
                x.Quantity,
                x.IsBillable,
                x.Notes,
                x.InvoiceId))
            .ToList()
            .AsReadOnly();

        return ResultDTO<IReadOnlyCollection<BillingEntryDTO>>.AsSuccess(dtos);
    }
}