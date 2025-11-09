using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Services.CreateService;

/// <summary>
/// Handles the creation of new services in response to a create service command.
/// </summary>
/// <remarks>This handler is typically used within a command processing pipeline to encapsulate the logic for
/// creating a service entity. It requires a valid database context and logger to function correctly. Thread safety is
/// determined by the underlying dependencies.</remarks>
/// <param name="dbContext">The database context used to access and persist service data. Must not be null.</param>
/// <param name="logger">The logger used to record informational and error messages during command handling. Must not be null.</param>
public class CreateServiceCommandHandler(SpheraDbContext dbContext, ILogger<CreateServiceCommandHandler> logger) : IHandler<CreateServiceCommand, ServiceDTO>
{
    /// <summary>
    /// Asynchronously handles the creation of a new service based on the specified command and user context.
    /// </summary>
    /// <param name="request">The command containing the details required to create the service. Must not be null.</param>
    /// <param name="context">The HTTP context associated with the current request, used to identify the user performing the operation. Must
    /// not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="IResultDTO{ServiceDTO}"/> indicating the outcome of the service creation. On success, the result contains
    /// the created service data; on failure, it contains error information.</returns>
    public async Task<IResultDTO<ServiceDTO>> HandleAsync(CreateServiceCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Inicializando criação do serviço.");
        var actor = context.User.GetUserId();

        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            DateTime? dueDate = request.DefaultDueInDays.HasValue ? DateTime.Today.AddDays(request.DefaultDueInDays.Value) : null;

            Service service = new(request.Name, request.Code, dueDate, actor);

            await dbContext.Services.AddAsync(service, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<ServiceDTO>.AsSuccess(service.ToDTO());
        }
        catch (DomainException e)
        {
            logger.LogError("Um erro ocorreu ao tentar criar o serviço", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ServiceDTO>.AsFailure(new FailureDTO(400, e.Message));
        }
        catch (Exception e)
        {
            logger.LogError("Um erro ocorreu ao tentar criar o serviço", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ServiceDTO>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar criar o serviço"));
        }
    }
}