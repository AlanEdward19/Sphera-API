using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Schedules.GetUserScheduleEvents;

public class GetUserScheduleEventsQueryHandler(SpheraDbContext dbContext, ILogger<GetUserScheduleEventsQueryHandler> logger)
    : IHandler<GetUserScheduleEventsQuery, IEnumerable<ScheduleEventDTO>>
{
    public async Task<IResultDTO<IEnumerable<ScheduleEventDTO>>> HandleAsync(GetUserScheduleEventsQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Recuperando eventos de agenda para UserId={UserId} StartAt={StartAt} EndAt={EndAt}", request.GetUserId(), request.StartAt, request.EndAt);

        IQueryable<ScheduleEvent> query = dbContext.ScheduleEvents.AsNoTracking();

        query = query.Where(s => s.UserId == request.GetUserId());

        if (request.StartAt.HasValue)
            query = query.Where(s => s.OccurredAt >= request.StartAt.Value);

        if (request.EndAt.HasValue)
            query = query.Where(s => s.OccurredAt <= request.EndAt.Value);

        var list = await query.OrderBy(s => s.OccurredAt)
            .Select(s => s.ToDTO())
            .ToListAsync(cancellationToken);

        return ResultDTO<IEnumerable<ScheduleEventDTO>>.AsSuccess(list);
    }
}
