using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.BilletConfigurations.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BilletConfigurations.ListBilletConfigurations;

public class ListBilletConfigurationsQueryHandler(
    SpheraDbContext dbContext,
    ILogger<ListBilletConfigurationsQueryHandler> logger)
    : IHandler<ListBilletConfigurationsQuery, IReadOnlyCollection<BilletConfigurationDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<BilletConfigurationDTO>>> HandleAsync(
        ListBilletConfigurationsQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listando BilletConfigurations CompanyName={CompanyName} BankCode={BankCode}", request.CompanyName, request.BankCode);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<IReadOnlyCollection<BilletConfigurationDTO>>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var query = dbContext.BilletConfigurations.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.CompanyName))
            query = query.Where(x => x.CompanyName.Contains(request.CompanyName));
        if (!string.IsNullOrWhiteSpace(request.BankCode))
            query = query.Where(x => x.BankCode == request.BankCode);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => BilletConfigurationDTO.FromEntity(x))
            .ToListAsync(cancellationToken);

        return ResultDTO<IReadOnlyCollection<BilletConfigurationDTO>>.AsSuccess(items);
    }
}

