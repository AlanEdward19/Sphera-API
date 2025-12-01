using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Services.DeleteService;

public class DeleteServiceCommandHandler(SpheraDbContext dbContext, ILogger<DeleteServiceCommandHandler> logger) : IHandler<DeleteServiceCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(DeleteServiceCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Iniciando exclusão para serviço: '{request.Id}'.");

        Service? service = await dbContext.Services.FindAsync([request.Id], cancellationToken);

        if (service is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, "Serviço não encontrado"));

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                dbContext.Services.Remove(service);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<bool>.AsSuccess(true);
            }
            catch (DomainException e)
            {
                logger.LogError($"Um erro ocorreu ao tentar excluir o serviço: '{request.Id}'.", e);
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(400, e.Message));
            }
            catch (Exception e)
            {
                logger.LogError($"Um erro ocorreu ao tentar excluir o serviço: '{request.Id}'.", e);
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar excluir o serviço."));
            }
        }, cancellationToken);
    }
}