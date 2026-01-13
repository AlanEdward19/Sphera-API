using Microsoft.EntityFrameworkCore;
using Sphera.API.Shared;
using Sphera.API.Billing.Billets.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Billets.CreateBillet;

public class CreateBilletCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateBilletCommandHandler> logger)
    : IHandler<CreateBilletCommand, BilletDTO>
{
    public async Task<IResultDTO<BilletDTO>> HandleAsync(
        CreateBilletCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando billet Bank={Bank} InstallmentId={InstallmentId} ConfigurationId={ConfigurationId} ClientId={ClientId} ", 
            request.Bank, request.InstallmentId, request.ConfigurationId, request.ClientId);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<BilletDTO>.AsFailure(new FailureDTO(401, "Unauthorized"));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = new Billet(request.Bank, userId, request.InstallmentId, request.ConfigurationId, request.ClientId);

                dbContext.Billets.Add(entity);
                await dbContext.SaveChangesAsync(cancellationToken);

                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<BilletDTO>.AsSuccess(BilletDTO.FromEntity(entity));
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<BilletDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<BilletDTO>.AsFailure(new FailureDTO(500, "Erro ao criar billet."));
            }
        });

        return result;
    }
}
