using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Services.GetServiceById;

/// <summary>
/// Handles queries to retrieve a service by its identifier and returns the result as a data transfer object (DTO).
/// </summary>
/// <param name="dbContext">The database context used to access service data.</param>
/// <param name="logger">The logger used to record diagnostic and operational information for this handler.</param>
public class GetServiceByIdQueryHandler(SpheraDbContext dbContext, ILogger<GetServiceByIdQueryHandler> logger) : IHandler<GetServiceByIdQuery, ServiceDTO>
{
    /// <summary>
    /// Handles the retrieval of a service by its identifier and returns the result as a data transfer object (DTO).
    /// </summary>
    /// <param name="request">The query containing the identifier of the service to retrieve.</param>
    /// <param name="context">The HTTP context associated with the current request.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IResultDTO{ServiceDTO}"/> indicating success with the service data if found, or failure with an error
    /// message if the service does not exist.</returns>
    public async Task<IResultDTO<ServiceDTO>> HandleAsync(GetServiceByIdQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        IQueryable<Service> query = dbContext.Services
            .AsNoTracking();

        Service? service = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (service is null)
            return ResultDTO<ServiceDTO>.AsFailure(new FailureDTO(400, "Serviço não encontrado"));

        return ResultDTO<ServiceDTO>.AsSuccess(service.ToDTO());
    }
}