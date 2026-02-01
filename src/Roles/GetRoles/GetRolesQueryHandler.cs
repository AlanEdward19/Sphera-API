using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Roles.DTOs;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Roles.GetRoles;

public class GetRolesQueryHandler(SpheraDbContext dbContext, ILogger<GetRolesQueryHandler> logger) : IHandler<GetRolesQuery, IEnumerable<RoleDTO>>
{
    public async Task<IResultDTO<IEnumerable<RoleDTO>>> HandleAsync(GetRolesQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando busca de roles. Parâmetros: Page={Page}, PageSize={PageSize}",request.Page, request.PageSize);

        IQueryable<Role> query = dbContext
            .Roles
            .AsNoTracking();

        var roles = await query
            .OrderBy(r => r.Id)
            .Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : 0))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Busca de roles concluída. {Count} registros encontrados.", roles.Count);

        return ResultDTO<IEnumerable<RoleDTO>>.AsSuccess(roles.Select(r => new RoleDTO(r.Id, r.Name)));
    }
}