using Sphera.API.External.Database;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using System.Data.Entity;

namespace Sphera.API.Partners.GetPartnerById;

/// <summary>
/// Handles queries to retrieve partner information by ID, returning the result as a data transfer object (DTO).
/// </summary>
/// <remarks>If the requested partner is not found, the handler returns a failure result with a 404 error code.
/// The returned PartnerDTO includes associated client information if the query specifies to include clients.</remarks>
/// <param name="dbContext">The database context used to access partner and related client data.</param>
/// <param name="logger">The logger instance used for logging query handling operations and errors.</param>
public class GetPartnerByIdQueryHandler(SpheraDbContext dbContext, ILogger<GetPartnerByIdQueryHandler> logger) : IHandler<GetPartnerByIdQuery, PartnerDTO>
{
    /// <summary>
    /// Asynchronously retrieves partner information by ID and returns the result as a data transfer object (DTO).
    /// </summary>
    /// <remarks>If the partner is not found, the result will indicate failure with a 404 error. The returned
    /// PartnerDTO will include client information if the query's includeClients flag is set to true.</remarks>
    /// <param name="request">The query containing the partner ID to retrieve and an optional flag indicating whether to include associated
    /// clients in the result.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A result object containing the partner data as a PartnerDTO if found; otherwise, a failure result with a 404
    /// error code.</returns>
    public async Task<ResultDTO<PartnerDTO>> HandleAsync(GetPartnerByIdQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Partner> query = dbContext.Partners
            .AsNoTracking()
            .Include(c => c.Contacts);

        bool includeClients = request.includeClients ?? false;

        if (includeClients)
            query.Include(x => x.Clients);

        Partner? partner = await query.FirstOrDefaultAsync(p => p.Id == request.Id);

        if(partner is null)
            return ResultDTO<PartnerDTO>.AsFailure(new FailureDTO(404, "Parceiro não encontrado"));

        return ResultDTO<PartnerDTO>.AsSuccess(partner.ToDTO(includeClients));
    }
}
