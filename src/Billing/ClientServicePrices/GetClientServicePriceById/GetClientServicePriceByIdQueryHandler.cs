using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.ClientServicePrices.GetClientServicePriceById;

public class GetClientServicePriceByIdQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetClientServicePriceByIdQueryHandler> logger)
    : IHandler<GetClientServicePriceByIdQuery, ClientServicePriceDTO>
{
    public async Task<IResultDTO<ClientServicePriceDTO>> HandleAsync(
        GetClientServicePriceByIdQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando ClientServicePrice {Id}", request.Id);

        var entity = await dbContext.ClientServicePrices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<ClientServicePriceDTO>.AsFailure(
                new FailureDTO(404, "Preço não encontrado."));

        var dto = new ClientServicePriceDTO(
            entity.Id,
            entity.ClientId,
            entity.ServiceId,
            entity.UnitPrice,
            entity.StartDate,
            entity.EndDate,
            entity.IsActive);

        return ResultDTO<ClientServicePriceDTO>.AsSuccess(dto);
    }
}