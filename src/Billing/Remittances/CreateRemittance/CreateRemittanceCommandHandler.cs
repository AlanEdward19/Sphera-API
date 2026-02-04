using Microsoft.EntityFrameworkCore;
using Sphera.API.Shared;
using Sphera.API.Billing.Billets;
using Sphera.API.Billing.Remittances.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Remittances.CreateRemittance;

public class CreateRemittanceCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateRemittanceCommandHandler> logger)
    : IHandler<CreateRemittanceCommand, RemittanceDTO>
{
    public async Task<IResultDTO<RemittanceDTO>> HandleAsync(
        CreateRemittanceCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando remessa FileName={FileName}", request.FileName);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = new Remittance(request.FileName, userId);

                if (request.BilletIds?.Count > 0)
                {
                    var billets = await dbContext.Billets
                        .Where(b => request.BilletIds.Contains(b.Id))
                        .ToListAsync(cancellationToken);

                    entity.AddBillets(billets);
                }

                dbContext.Remittances.Add(entity);
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
                return ResultDTO<RemittanceDTO>.AsFailure(new FailureDTO(500, "Erro ao criar remessa."));
            }
        });

        return result;
    }
}
