using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Schedules.UpdateScheduleEvent;

public class UpdateScheduleEventCommandHandler(SpheraDbContext dbContext, ILogger<UpdateScheduleEventCommandHandler> logger) : IHandler<UpdateScheduleEventCommand, ScheduleEventDTO>
{
    public async Task<IResultDTO<ScheduleEventDTO>> HandleAsync(UpdateScheduleEventCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando atualiza��o de evento de agenda {Id}", request.GetId());

        var entity = await dbContext.ScheduleEvents.FindAsync(new object[] { request.GetId() }, cancellationToken);

        if (entity is null)
            return ResultDTO<ScheduleEventDTO>.AsFailure(new FailureDTO(404, "Evento n�o encontrado"));

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                var actor = context.User.GetUserId();
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                entity.Update(request.OccurredAt, request.EventType, request.UserId, request.ClientId, request.Notes, actor);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<ScheduleEventDTO>.AsSuccess(entity.ToDTO());
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<ScheduleEventDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "Erro ao atualizar evento de agenda");
                return ResultDTO<ScheduleEventDTO>.AsFailure(new FailureDTO(500, "Erro ao atualizar evento de agenda"));
            }
        }, cancellationToken);
    }
}
