using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.BilletConfigurations.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BilletConfigurations.UpdateBilletConfiguration;

public class UpdateBilletConfigurationCommandHandler(
    SpheraDbContext dbContext,
    ILogger<UpdateBilletConfigurationCommandHandler> logger)
    : IHandler<UpdateBilletConfigurationCommand, BilletConfigurationDTO>
{
    public async Task<IResultDTO<BilletConfigurationDTO>> HandleAsync(
        UpdateBilletConfigurationCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Atualizando BilletConfiguration {Id}", request.GetId());

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = await dbContext.BilletConfigurations
                    .FirstOrDefaultAsync(x => x.Id == request.GetId(), cancellationToken);
                if (entity is null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(404, "BilletConfiguration not found"));
                }

                entity.Update(request, userId);
                
                await dbContext.SaveChangesAsync(cancellationToken);

                await dbContext.Database.CommitTransactionAsync(cancellationToken);
                return ResultDTO<BilletConfigurationDTO>.AsSuccess(BilletConfigurationDTO.FromEntity(entity));
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(500, "Erro ao atualizar BilletConfiguration."));
            }
        });

        return result;
    }
}
