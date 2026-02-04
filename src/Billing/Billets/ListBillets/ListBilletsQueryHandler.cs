using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Billets.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Billets.ListBillets;

public class ListBilletsQueryHandler(
    SpheraDbContext dbContext,
    ILogger<ListBilletsQueryHandler> logger)
    : IHandler<ListBilletsQuery, IReadOnlyCollection<BilletDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<BilletDTO>>> HandleAsync(
        ListBilletsQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listando billets ClientId={ClientId} InstallmentId={InstallmentId}", request.ClientId, request.InstallmentId);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<IReadOnlyCollection<BilletDTO>>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var query = dbContext.Billets.AsNoTracking().AsQueryable();

        if (request.ClientId.HasValue)
            query = query.Where(b => b.ClientId == request.ClientId.Value);
        if (request.InstallmentId.HasValue)
            query = query.Where(b => b.InstallmentId == request.InstallmentId.Value);

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => BilletDTO.FromEntity(b))
            .ToListAsync(cancellationToken);

        return ResultDTO<IReadOnlyCollection<BilletDTO>>.AsSuccess(items);
    }
}
