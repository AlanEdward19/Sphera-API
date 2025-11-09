using Microsoft.EntityFrameworkCore;
using Sphera.API.Clients.GetClients;
using Sphera.API.External.Database;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Partners.GetPartners;

/// <summary>
/// Handles queries to retrieve a paginated list of partners based on specified filtering and search criteria.
/// </summary>
/// <remarks>Use this handler to obtain partner information with optional filtering, searching, and pagination.
/// Related client data can be included in the results if requested by the query parameters.</remarks>
/// <param name="dbContext">The database context used to access partner and related data.</param>
/// <param name="logger">The logger instance used for logging query handling operations.</param>
public class GetPartnersQueryHandler(SpheraDbContext dbContext, ILogger<GetClientsQueryHandler> logger) : IHandler<GetPartnersQuery, IEnumerable<PartnerDTO>>
{
    /// <summary>
    /// Asynchronously retrieves a paginated list of partners matching the specified query criteria.
    /// </summary>
    /// <remarks>The returned collection reflects the filters and pagination specified in the query. If no
    /// partners match the criteria, the collection will be empty. The result may include related client information if
    /// requested.</remarks>
    /// <param name="request">The query parameters used to filter, search, and paginate the list of partners. Must not be null.</param>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a successful result with an
    /// enumerable collection of partner data transfer objects matching the query.</returns>
    public async Task<IResultDTO<IEnumerable<PartnerDTO>>> HandleAsync(GetPartnersQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        //TODO: Colocar Logs
        IQueryable<Partner> query = dbContext
            .Partners
            .AsNoTracking()
            .Include(x => x.Contacts);

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Cnpj))
            query = query.Where(c => c.Cnpj != null && c.Cnpj.Value == request.Cnpj);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchLower = request.Search!.ToLower();
            query = query.Where(c => EF.Functions.Like((c.LegalName ?? string.Empty).ToLower(), $"%{searchLower}%"));
        }

        bool includeClients = request.IncludeClients.HasValue && request.IncludeClients.Value;

        if (includeClients)
            query = query
                .Include(x => x.Clients)
                .ThenInclude(x => x.Contacts);

        List<Partner> partners = await query
            .Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : 0))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        if (includeClients)
            return ResultDTO<IEnumerable<PartnerWithClientsDTO>>.AsSuccess(partners.Select(p =>
                (PartnerWithClientsDTO)p.ToDTO(includeClients)));

        return ResultDTO<IEnumerable<PartnerDTO>>.AsSuccess(partners.Select(p => p.ToDTO(includeClients)));
    }
}
