using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Schedules.CreateScheduleEvent;

/// <summary>
/// Handler responsible for creating schedule events.
/// </summary>
/// <param name="dbContext"></param>
/// <param name="logger"></param>
public class CreateScheduleEventCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateScheduleEventCommandHandler> logger)
    : IHandler<CreateScheduleEventCommand, ScheduleEventDTO>
{
    /// <summary>
    /// Creates a new schedule event based on the specified command and returns the result asynchronously.
    /// </summary>
    /// <param name="request">The command containing the details required to create the schedule event. Cannot be null.</param>
    /// <param name="context">The HTTP context associated with the current request. Used to identify the user performing the operation. Cannot
    /// be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IResultDTO{ScheduleEventDTO}"/> indicating the outcome of the operation. On success, the result includes
    /// the created schedule event; on failure, it contains error details.</returns>
    public async Task<IResultDTO<ScheduleEventDTO>> HandleAsync(CreateScheduleEventCommand request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Criando evento de agenda para UserId={UserId} ClientId={ClientId} OccurredAt={OccurredAt}",
            request.UserId, request.ClientId, request.OccurredAt);
        
        try
        {
            var actor = context.User.GetUserId();
            
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var entity = new ScheduleEvent(request.OccurredAt, request.UserId, request.ClientId, actor, request.Notes);

            await dbContext.ScheduleEvents.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<ScheduleEventDTO>.AsSuccess(entity.ToDTO());
        }
        catch (DomainException e)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ScheduleEventDTO>.AsFailure(new FailureDTO(400, e.Message));
        }
        catch (Exception e)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            logger.LogError(e, "Ocorreu um erro ao tentar criar um evento para o usuário.");
            return ResultDTO<ScheduleEventDTO>.AsFailure(new FailureDTO(500,
                "Ocorreu um erro ao tentar criar um evento para o usuário."));
        }
    }
}