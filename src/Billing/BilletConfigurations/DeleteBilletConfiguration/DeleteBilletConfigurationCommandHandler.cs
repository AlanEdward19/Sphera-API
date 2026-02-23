using Microsoft.EntityFrameworkCore;
using Sphera.API.Shared;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BilletConfigurations.DeleteBilletConfiguration;

public class DeleteBilletConfigurationCommandHandler(
    SpheraDbContext dbContext,
    ILogger<DeleteBilletConfigurationCommandHandler> logger)
    : IHandler<DeleteBilletConfigurationCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(
        DeleteBilletConfigurationCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deletando BilletConfiguration {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<bool>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = await dbContext.BilletConfigurations
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (entity is null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<bool>.AsFailure(new FailureDTO(404, "BilletConfiguration not found"));
                }

                dbContext.BilletConfigurations.Remove(entity);
                await dbContext.SaveChangesAsync(cancellationToken);

                await dbContext.Database.CommitTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsSuccess(true);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao deletar BilletConfiguration."));
            }
        });

        return result;
    }
}
