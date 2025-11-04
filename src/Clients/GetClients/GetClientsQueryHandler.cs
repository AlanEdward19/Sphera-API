using Sphera.API.Clients.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using System.Data.Entity;

namespace Sphera.API.Clients.GetClients;

/// <summary>
/// Provides functionality to handle queries for retrieving client records based on specified filter criteria.
/// </summary>
/// <remarks>This handler supports filtering clients by partner ID, status, CNPJ, and search terms, and can
/// optionally include associated partner details in the results. The operation is performed asynchronously and does not
/// track changes to the retrieved entities.</remarks>
/// <param name="dbContext">The database context used to access client and partner data.</param>
/// <param name="logger">The logger instance used for recording diagnostic and operational information.</param>
public class GetClientsQueryHandler(SpheraDbContext dbContext, ILogger<GetClientsQueryHandler> logger) : IHandler<GetClientsQuery, IEnumerable<ClientDTO>>
{
    /// <summary>
    /// Handles the retrieval of client records based on the specified query parameters.
    /// </summary>
    /// <remarks>The returned client data may include associated partner information if requested. The
    /// operation is performed asynchronously and does not track changes to the retrieved entities.</remarks>
    /// <param name="request">The query object containing filter criteria such as partner ID, status, CNPJ, search term, and an option to
    /// include partner details.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A result object containing a collection of client data transfer objects that match the query criteria. The
    /// collection will be empty if no clients are found.</returns>
    public async Task<ResultDTO<IEnumerable<ClientDTO>>> HandleAsync(GetClientsQuery request, CancellationToken cancellationToken)
    {
        //TODO: Colocar Logs
        IQueryable<Client> query = dbContext
            .Clients
            .AsQueryable()
            .Include(x => x.Contacts);

        if (request.PartnerId.HasValue)
            query = query.Where(c => c.PartnerId == request.PartnerId.Value);

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Cnpj))
            query = query.Where(c => c.Cnpj.Value == request.Cnpj);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query
                .Where(c =>
                c.TradeName.Contains(request.Search!, StringComparison.OrdinalIgnoreCase) ||
                c.LegalName.Contains(request.Search!, StringComparison.OrdinalIgnoreCase));

        bool includePartner = request.IncludePartner.HasValue && request.IncludePartner.Value;

        if (includePartner)
            query = query.Include(x => x.Partner);

        List<Client> clients = await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        IEnumerable<ClientDTO> clientDTOs = clients.Select(c => c.ToDTO(request.IncludePartner.HasValue && request.IncludePartner.Value));

        return ResultDTO<IEnumerable<ClientDTO>>.AsSuccess(clientDTOs);
    }
}
