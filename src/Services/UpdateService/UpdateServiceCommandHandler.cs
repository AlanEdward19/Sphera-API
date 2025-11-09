using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Services.UpdateService;

public class UpdateServiceCommandHandler(SpheraDbContext dbContext, ILogger<UpdateServiceCommandHandler> logger) : IHandler<UpdateServiceCommand, ServiceDTO>
{
    public async Task<IResultDTO<ServiceDTO>> HandleAsync(UpdateServiceCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Iniciando atualização para serviço: '{request.GetId()}'.");
        Service? service = await dbContext.Services.FindAsync([request.GetId()], cancellationToken);
        
        Guid actor = context.User.GetUserId();
        
        if (service is null)
            return ResultDTO<ServiceDTO>.AsFailure(new FailureDTO(400, "Serviço não encontrado"));
        
        try
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

            DateTime? dueDate = request.DefaultDueInDays.HasValue ? DateTime.Today.AddDays(request.DefaultDueInDays.Value) : null;

            service.Update(request.Name, dueDate, actor);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<ServiceDTO>.AsSuccess(service.ToDTO());
        }
        catch (DomainException e)
        {
            logger.LogError($"Um erro ocorreu ao tentar atualizar o serviço: '{request.GetId()}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ServiceDTO>.AsFailure(new FailureDTO(400, e.Message));
        }
        catch (Exception e)
        {
            logger.LogError($"Um erro ocorreu ao tentar atualizar o serviço: '{request.GetId()}'.", e);
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<ServiceDTO>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar atualizar o serviço."));
        }
    }
}