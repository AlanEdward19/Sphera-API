using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.BilletConfigurations.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BilletConfigurations.CreateBilletConfiguration;

public class CreateBilletConfigurationCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateBilletConfigurationCommandHandler> logger)
    : IHandler<CreateBilletConfigurationCommand, BilletConfigurationDTO>
{
    public async Task<IResultDTO<BilletConfigurationDTO>> HandleAsync(
        CreateBilletConfigurationCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando BilletConfiguration CompanyName={CompanyName}", request.CompanyName);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = new BilletConfiguration(request, userId);

                dbContext.BilletConfigurations.Add(entity);
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
                return ResultDTO<BilletConfigurationDTO>.AsFailure(new FailureDTO(500, "Erro ao criar BilletConfiguration."));
            }
        });

        return result;
    }
}
