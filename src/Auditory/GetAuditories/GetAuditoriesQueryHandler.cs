using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Auditory.GetAuditories;

public class GetAuditoriesQueryHandler(SpheraDbContext dbContext, ILogger<GetAuditoriesQueryHandler> logger)
    : IHandler<GetAuditoriesQuery, IEnumerable<AuditoryDTO>>
{
    public async Task<IResultDTO<IEnumerable<AuditoryDTO>>> HandleAsync(GetAuditoriesQuery request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando busca de auditórios.");

        IQueryable<AuditEntry> query = dbContext.AuditEntries
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

        query = query.Where(x => !x.EntityType.Contains("ValueObject") && !x.EntityType.Equals("Contact"));

        var intermediate = await query
            .Select(a => new
            {
                a.Id,
                a.OccurredAt,
                a.ActorId,
                a.Action,
                a.EntityType,
                a.EntityId,
                a.RequestIp,
                ActorEmail = a.Actor.Email.Address,
                ActorName = a.Actor.Name
            })
            .ToListAsync(cancellationToken);

        var result = new List<AuditoryDTO>(intermediate.Count);

        var clientIds = intermediate.Where(i => i.EntityType == "Client" && i.EntityId.HasValue)
            .Select(i => i.EntityId!.Value).Distinct().ToList();
        var partnerIds = intermediate.Where(i => i.EntityType == "Partner" && i.EntityId.HasValue)
            .Select(i => i.EntityId!.Value).Distinct().ToList();
        var serviceIds = intermediate.Where(i => i.EntityType == "Service" && i.EntityId.HasValue)
            .Select(i => i.EntityId!.Value).Distinct().ToList();
        var userIds = intermediate.Where(i => i.EntityType == "User" && i.EntityId.HasValue)
            .Select(i => i.EntityId!.Value).Distinct().ToList();
        var documentIds = intermediate.Where(i => i.EntityType == "Document" && i.EntityId.HasValue)
            .Select(i => i.EntityId!.Value).Distinct().ToList();
        var scheduleEventIds = intermediate.Where(i => i.EntityType == "ScheduleEvent" && i.EntityId.HasValue)
            .Select(i => i.EntityId!.Value).Distinct().ToList();

// fetch in bulk and create lookup dictionaries (use object as key to avoid pinning a specific id type)
        var clientMap = clientIds.Any()
            ? (await dbContext.Clients.AsNoTracking()
                .Where(c => clientIds.Contains(c.Id))
                .Select(c => new { c.Id, c.LegalName })
                .ToListAsync(cancellationToken))
            .ToDictionary(x => (object)x.Id, x => x.LegalName)
            : new Dictionary<object, string?>();

        var partnerMap = partnerIds.Any()
            ? (await dbContext.Partners.AsNoTracking()
                .Where(p => partnerIds.Contains(p.Id))
                .Select(p => new { p.Id, p.LegalName })
                .ToListAsync(cancellationToken))
            .ToDictionary(x => (object)x.Id, x => x.LegalName)
            : new Dictionary<object, string?>();

        var serviceMap = serviceIds.Any()
            ? (await dbContext.Services.AsNoTracking()
                .Where(s => serviceIds.Contains(s.Id))
                .Select(s => new { s.Id, s.Name })
                .ToListAsync(cancellationToken))
            .ToDictionary(x => (object)x.Id, x => x.Name)
            : new Dictionary<object, string?>();

        var userMap = userIds.Any()
            ? (await dbContext.Users.AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Name })
                .ToListAsync(cancellationToken))
            .ToDictionary(x => (object)x.Id, x => x.Name)
            : new Dictionary<object, string?>();

        var documentMap = documentIds.Any()
            ? (await dbContext.Documents.AsNoTracking()
                .Where(d => documentIds.Contains(d.Id))
                .Select(d => new { d.Id, d.FileName })
                .ToListAsync(cancellationToken))
            .ToDictionary(x => (object)x.Id, x => x.FileName)
            : new Dictionary<object, string?>();

        var scheduleEventMap = scheduleEventIds.Any()
            ? (await dbContext.ScheduleEvents.AsNoTracking()
                .Where(se => scheduleEventIds.Contains(se.Id))
                .Select(se => new { se.Id, se.OccurredAt })
                .ToListAsync(cancellationToken))
            .ToDictionary(x => (object)x.Id, x => x.OccurredAt)
            : new Dictionary<object, DateTime>();

        foreach (var item in intermediate)
        {
            string? entityName = null;

            if (item.EntityId.HasValue && !string.IsNullOrEmpty(item.EntityType))
            {
                var key = (object)item.EntityId.Value;

                switch (item.EntityType)
                {
                    case "Client":
                        clientMap.TryGetValue(key, out entityName);
                        break;

                    case "Partner":
                        partnerMap.TryGetValue(key, out entityName);
                        break;

                    case "Service":
                        serviceMap.TryGetValue(key, out entityName);
                        break;

                    case "User":
                        userMap.TryGetValue(key, out entityName);
                        break;

                    case "Document":
                        documentMap.TryGetValue(key, out entityName);
                        break;

                    case "ScheduleEvent":
                        if (scheduleEventMap.TryGetValue(key, out var occurredAt))
                            entityName = occurredAt.ToString(CultureInfo.CurrentCulture);
                        break;

                    default:
                        entityName = null;
                        break;
                }
            }

            var dto = new AuditoryDTO(
                item.Id,
                item.OccurredAt,
                item.ActorId,
                item.Action,
                item.EntityType,
                entityName,
                item.EntityId,
                item.RequestIp,
                item.ActorEmail,
                item.ActorName
            );

            result.Add(dto);
        }

        return ResultDTO<IEnumerable<AuditoryDTO>>.AsSuccess(result);
    }
}