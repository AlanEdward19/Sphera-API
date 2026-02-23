using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.BilletConfigurations.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BilletConfigurations.GetBilletConfigurationById;

public class GetBilletConfigurationByIdQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetBilletConfigurationByIdQueryHandler> logger)
    : IHandler<GetBilletConfigurationByIdQuery, BilletConfigurationDTO>
{
    public async Task<IResultDTO<BilletConfigurationDTO>> HandleAsync(
        GetBilletConfigurationByIdQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando BilletConfiguration {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var entity = await dbContext.BilletConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(404, "BilletConfiguration not found"));

        return ResultDTO<BilletConfigurationDTO>.AsSuccess(BilletConfigurationDTO.FromEntity(entity));
    }
}

