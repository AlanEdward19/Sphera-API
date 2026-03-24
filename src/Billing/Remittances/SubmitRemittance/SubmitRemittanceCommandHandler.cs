using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Remittances.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Remittances.SubmitRemittance;

public class SubmitRemittanceCommandHandler(
    SpheraDbContext dbContext,
    [FromKeyedServices("billing")] IStorage storage,
    ILogger<SubmitRemittanceCommandHandler> logger)
    : IHandler<SubmitRemittanceCommand, RemittanceDTO>
{
    public async Task<IResultDTO<RemittanceDTO>> HandleAsync(
        SubmitRemittanceCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Submetendo remessa {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = await dbContext.Remittances
                    .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

                if (entity is null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(404, "Remittance not found"));
                }

                if (entity.IsSubmitted)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(409, "Remittance already submitted"));
                }

                if (string.IsNullOrEmpty(entity.FileName) || !(await storage.ExistsAsync(entity.FileName, cancellationToken)))
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(404, "Remittance file not generated or not found"));
                }

                entity.MarkAsSubmitted(context.User.GetUserId());

                await dbContext.SaveChangesAsync(cancellationToken);

                await dbContext.Database.CommitTransactionAsync(cancellationToken);
                return ResultDTO<RemittanceDTO>.AsSuccess(RemittanceDTO.FromEntity(entity));
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(500, "Erro ao submeter remessa."));
            }
        });

        return result;
    }
}
