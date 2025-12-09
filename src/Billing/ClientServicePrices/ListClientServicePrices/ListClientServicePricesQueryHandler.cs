using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.ClientServicePrices.ListClientServicePrices;

public class ListClientServicePricesQueryHandler(
    SpheraDbContext dbContext,
    ILogger<ListClientServicePricesQueryHandler> logger)
    : IHandler<ListClientServicePricesQuery, IReadOnlyCollection<ClientServicePriceDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<ClientServicePriceDTO>>> HandleAsync(
        ListClientServicePricesQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listando ClientServicePrices");

        IQueryable<ClientServicePrice> query = dbContext.ClientServicePrices.AsNoTracking();

        if (request.ClientId.HasValue)
            query = query.Where(x => x.ClientId == request.ClientId.Value);

        if (request.ServiceId.HasValue)
            query = query.Where(x => x.ServiceId == request.ServiceId.Value);

        if (request.OnlyActive)
            query = query.Where(x => x.IsActive);

        var list = await query
            .OrderBy(x => x.ClientId)
            .ThenBy(x => x.ServiceId)
            .ThenByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);

        var dtos = list
            .Select(x => new ClientServicePriceDTO(
                x.Id,
                x.ClientId,
                x.ServiceId,
                x.UnitPrice,
                x.StartDate,
                x.EndDate,
                x.IsActive))
            .ToList()
            .AsReadOnly();

        return ResultDTO<IReadOnlyCollection<ClientServicePriceDTO>>.AsSuccess(dtos);
    }
}