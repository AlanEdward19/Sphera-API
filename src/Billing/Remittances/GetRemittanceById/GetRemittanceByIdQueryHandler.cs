using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Billets;
using Sphera.API.Billing.Remittances.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Remittances.GetRemittanceById;

public class GetRemittanceByIdQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetRemittanceByIdQueryHandler> logger)
    : IHandler<GetRemittanceByIdQuery, RemittanceDTO>
{
    public async Task<IResultDTO<RemittanceDTO>> HandleAsync(
        GetRemittanceByIdQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando remessa {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var entity = await dbContext.Remittances
            .Include(r => r.Billets)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(404, "Remittance not found"));

        return ResultDTO<RemittanceDTO>.AsSuccess(RemittanceDTO.FromEntity(entity));
    }
}
