using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Schedules.DeleteScheduleEvent;

public class DeleteScheduleEventCommandHandler(SpheraDbContext dbContext, ILogger<DeleteScheduleEventCommandHandler> logger) : IHandler<DeleteScheduleEventCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(DeleteScheduleEventCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removendo evento de agenda {Id}", request.GetId());

        var entity = await dbContext.ScheduleEvents.FindAsync(new object[] { request.GetId() }, cancellationToken);

        if (entity is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Evento não encontrado"));

        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            dbContext.ScheduleEvents.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (Exception ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "Erro ao deletar evento de agenda");
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao deletar evento de agenda"));
        }
    }
}
