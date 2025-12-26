using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Services.GetServices;

/// <summary>
/// Handles queries to retrieve a collection of services based on specified filtering criteria.
/// </summary>
/// <param name="dbContext">The database context used to access and query service data.</param>
/// <param name="logger">The logger instance used to record diagnostic and operational information for this handler.</param>
public class GetServicesQueryHandler(SpheraDbContext dbContext, ILogger<GetServicesQueryHandler> logger) : IHandler<GetServicesQuery, IEnumerable<ServiceDTO>>
{
    /// <summary>
    /// Asynchronously retrieves a collection of services that match the specified query criteria.
    /// </summary>
    /// <param name="request">The query parameters used to filter the list of services. May include name, code, and active status filters.</param>
    /// <param name="context">The HTTP context for the current request. Used to access request-specific information if needed.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IResultDTO{IEnumerable{ServiceDTO}}"/> with the list of matching services. The collection will be empty if
    /// no services match the criteria.</returns>
    public async Task<IResultDTO<IEnumerable<ServiceDTO>>> HandleAsync(GetServicesQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando recuperação da lista de serviços.");

        var services = dbContext.Services.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            services = services.Where(s => EF.Functions.Like(s.Name.ToLower(), $"%{request.Name.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(request.Code))
            services = services.Where(s => EF.Functions.Like(s.Code.ToLower(), $"%{request.Code.ToLower()}%"));

        if(request.IsActive.HasValue)
            services = services.Where(s => s.IsActive == request.IsActive.Value);

        var serviceDTOs = await services
            .Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : 0))
            .Take(request.PageSize)
            .Select(s => s.ToDTO())
            .ToListAsync(cancellationToken);

        return ResultDTO<IEnumerable<ServiceDTO>>.AsSuccess(serviceDTOs);
    }
}