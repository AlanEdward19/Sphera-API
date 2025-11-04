using Sphera.API.Clients.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using System.Data.Entity;

namespace Sphera.API.Clients.GetClientById;

/// <summary>
/// Handles queries to retrieve a client by its unique identifier and returns the result as a data transfer object
/// (DTO).
/// </summary>
/// <remarks>If the client is not found, the handler returns a failure result with a 404 status code. If the query
/// requests partner information, the returned DTO will include partner details.</remarks>
/// <param name="dbContext">The database context used to access client and related data.</param>
/// <param name="logger">The logger used to record diagnostic and operational information for this handler.</param>
public class GetClientByIdQueryHandler(SpheraDbContext dbContext, ILogger<GetClientByIdQueryHandler> logger) : IHandler<GetClientByIdQuery, ClientDTO>
{
    /// <summary>
    /// Retrieves a client by its unique identifier and returns the result as a data transfer object (DTO).
    /// </summary>
    /// <remarks>If the client is not found, the result will indicate failure with a 404 status code. If the
    /// query requests partner information, the returned DTO will include partner details.</remarks>
    /// <param name="request">The query containing the client identifier and options for including related partner information.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A result object containing the client DTO if found; otherwise, a failure result with a 404 error code.</returns>
    public async Task<ResultDTO<ClientDTO>> HandleAsync(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Client> query = dbContext.Clients
            .AsNoTracking()
            .Include(c => c.Contacts);

        bool includePartner = request.includePartner.HasValue && request.includePartner.Value;

        if (includePartner)
            query = query.Include(c => c.Partner);

        Client? client = await query.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (client is null)
            return ResultDTO<ClientDTO>.AsFailure(new FailureDTO(404, "Cliente não encontrado"));

        return ResultDTO<ClientDTO>.AsSuccess(client.ToDTO(includePartner));
    }
}
