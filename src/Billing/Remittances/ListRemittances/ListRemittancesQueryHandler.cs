using System.Data.Entity;
using Sphera.API.Billing.Remittances.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Remittances.ListRemittances;

public class ListRemittancesQueryHandler(
    SpheraDbContext dbContext,
    ILogger<ListRemittancesQueryHandler> logger)
    : IHandler<ListRemittancesQuery, IReadOnlyCollection<RemittanceDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<RemittanceDTO>>> HandleAsync(
        ListRemittancesQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listando remessas IsSubmitted={IsSubmitted}", request.IsSubmitted);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<IReadOnlyCollection<RemittanceDTO>>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var query = dbContext.Remittances
            .Include(r => r.Billets)
            .AsQueryable();

        if (request.IsSubmitted.HasValue)
            query = query.Where(r => r.IsSubmitted == request.IsSubmitted.Value);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => RemittanceDTO.FromEntity(r))
            .ToListAsync(cancellationToken);

        return ResultDTO<IReadOnlyCollection<RemittanceDTO>>.AsSuccess(items);
    }
}

