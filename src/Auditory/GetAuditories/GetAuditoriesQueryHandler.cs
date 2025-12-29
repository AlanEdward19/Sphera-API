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
        string? EntityName,
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
        sql.AppendLine("SELECT ae.Id, ae.OccurredAt, ae.ActorId, ae.Action, ae.EntityType, ae.EntityName, ae.EntityId, ae.RequestIp, u.Email AS ActorEmail, u.Name AS ActorName");
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
            
            sql.AppendLine($"AND (ae.EntityType LIKE @p{pIndex} OR ae.Action LIKE @p{pIndex} OR u.Name LIKE @p{pIndex} OR u.Email LIKE @p{pIndex} OR ae.EntityName LIKE @p{pIndex})");
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

        foreach (var item in intermediate)
        {
            var dto = new AuditoryDTO(
                item.Id,
                item.OccurredAt,
                item.ActorId,
                item.Action,
                item.EntityType,
                item.EntityName,
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