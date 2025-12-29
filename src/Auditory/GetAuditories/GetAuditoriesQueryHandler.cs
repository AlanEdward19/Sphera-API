using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.ValueObjects;
using System.Text;

namespace Sphera.API.Auditory.GetAuditories;

public class GetAuditoriesQueryHandler(SpheraDbContext dbContext, ILogger<GetAuditoriesQueryHandler> logger)
    : IHandler<GetAuditoriesQuery, IEnumerable<AuditoryDTO>>
{
    // private record to map raw SQL projection
    private sealed record IntermediateAuditory(
        long Id,
        DateTime OccurredAt,
        Guid ActorId,
        string Action,
        string EntityType,
        Guid? EntityId,
        string RequestIp,
        string? ActorEmail,
        string? ActorName
    );

    public async Task<IResultDTO<IEnumerable<AuditoryDTO>>> HandleAsync(GetAuditoriesQuery request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando busca de auditórios");
        
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ae.Id, ae.OccurredAt, ae.ActorId, ae.Action, ae.EntityType, ae.EntityId, ae.RequestIp, u.Email AS ActorEmail, u.Name AS ActorName");
        sql.AppendLine("FROM AuditEntries ae");
        sql.AppendLine("LEFT JOIN Users u ON ae.ActorId = u.Id");
        sql.AppendLine("WHERE 1 = 1");

        var parameters = new List<SqlParameter>();
        int pIndex = 0;

        if (request.Id.HasValue)
        {
            sql.AppendLine($"AND ae.Id = @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.Id.Value));
            pIndex++;
        }

        if (request.OccurredAtStart.HasValue)
        {
            sql.AppendLine($"AND ae.OccurredAt >= @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.OccurredAtStart.Value));
            pIndex++;
        }

        if (request.OccurredAtEnd.HasValue)
        {
            sql.AppendLine($"AND ae.OccurredAt <= @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.OccurredAtEnd.Value));
            pIndex++;
        }

        if (request.ActorId.HasValue)
        {
            sql.AppendLine($"AND ae.ActorId = @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.ActorId.Value));
            pIndex++;
        }

        if (!string.IsNullOrEmpty(request.Action))
        {
            sql.AppendLine($"AND ae.Action = @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.Action));
            pIndex++;
        }

        if (!string.IsNullOrEmpty(request.EntityType))
        {
            sql.AppendLine($"AND ae.EntityType = @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.EntityType));
            pIndex++;
        }

        if (request.EntityId.HasValue)
        {
            sql.AppendLine($"AND ae.EntityId = @p{pIndex}");
            parameters.Add(new SqlParameter($"@p{pIndex}", request.EntityId.Value));
            pIndex++;
        }

        if (!string.IsNullOrEmpty(request.Search))
        {
            var pattern = $"%{request.Search!.Trim()}%";
            
            sql.AppendLine($"AND (ae.EntityType LIKE @p{pIndex} OR ae.Action LIKE @p{pIndex} OR u.Name LIKE @p{pIndex} OR u.Email LIKE @p{pIndex})");
            parameters.Add(new SqlParameter($"@p{pIndex}", pattern));
            pIndex++;
        }
        
        sql.AppendLine("AND (ae.EntityType NOT LIKE @p_exclude_valueobject)");
        parameters.Add(new SqlParameter("@p_exclude_valueobject", "%ValueObject%"));
        sql.AppendLine("AND (ae.EntityType <> @p_exclude_contact)");
        parameters.Add(new SqlParameter("@p_exclude_contact", "Contact"));
        
        sql.AppendLine("ORDER BY ae.Id DESC");

        var pageSize = Math.Max(1, request.PageSize);
        var page = request.Page > 0 ? request.Page - 1 : 0;
        var offset = pageSize * page;

        sql.AppendLine($"OFFSET @p{pIndex} ROWS FETCH NEXT @p{pIndex + 1} ROWS ONLY");
        parameters.Add(new SqlParameter($"@p{pIndex}", offset));
        parameters.Add(new SqlParameter($"@p{pIndex + 1}", pageSize));
        
        var intermediate = await dbContext.Database
            .SqlQueryRaw<IntermediateAuditory>(sql.ToString(), parameters.ToArray())
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