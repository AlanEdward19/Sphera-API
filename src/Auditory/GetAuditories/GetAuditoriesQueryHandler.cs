using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Auditory.GetAuditories;

public class GetAuditoriesQueryHandler(SpheraDbContext dbContext, ILogger<GetAuditoriesQueryHandler> logger)
: IHandler<GetAuditoriesQuery, IEnumerable<AuditoryDTO>>
{
    public async Task<IResultDTO<IEnumerable<AuditoryDTO>>> HandleAsync(GetAuditoriesQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando busca de auditórios.");
        
        IQueryable<AuditEntry> query = dbContext.
            AuditEntries
            .AsNoTracking()
            .Include(a => a.Actor);
        
        if (request.Id.HasValue)
            query = query.Where(a => a.Id == request.Id.Value);
        
        if (request.OccurredAtStart.HasValue)
            query = query.Where(a => a.OccurredAt >= request.OccurredAtStart.Value);

        if (request.OccurredAtEnd.HasValue)
            query = query.Where(a => a.OccurredAt <= request.OccurredAtEnd.Value);

        if (request.ActorId.HasValue)
            query = query.Where(a => a.ActorId == request.ActorId.Value);

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(a => a.Action == request.Action);

        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(a => a.EntityType == request.EntityType);

        if (request.EntityId.HasValue)
            query = query.Where(a => a.EntityId == request.EntityId.Value);

        var result = await query
            .Select(a => new AuditoryDTO(
            
                a.Id,
                a.OccurredAt,
                a.ActorId,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.RequestIp,
                a.Actor.Email.Address
            ))
            .ToListAsync(cancellationToken);

        return ResultDTO<IEnumerable<AuditoryDTO>>.AsSuccess(result);
    }
}