using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Billets.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Billets.GetBilletById;

public class GetBilletByIdQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetBilletByIdQueryHandler> logger)
    : IHandler<GetBilletByIdQuery, BilletDTO>
{
    public async Task<IResultDTO<BilletDTO>> HandleAsync(
        GetBilletByIdQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando billet {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<BilletDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var entity = await dbContext.Billets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<BilletDTO>.AsFailure(new FailureDTO(404, "Billet not found"));

        return ResultDTO<BilletDTO>.AsSuccess(BilletDTO.FromEntity(entity));
    }
}
