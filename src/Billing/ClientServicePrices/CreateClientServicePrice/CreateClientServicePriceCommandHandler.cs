using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.ClientServicePrices.CreateClientServicePrice;

public class CreateClientServicePriceCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateClientServicePriceCommandHandler> logger)
    : IHandler<CreateClientServicePriceCommand, ClientServicePriceDTO>
{
    public async Task<IResultDTO<ClientServicePriceDTO>> HandleAsync(
        CreateClientServicePriceCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando preço para Cliente {ClientId}, Serviço {ServiceId}",
            request.ClientId, request.ServiceId);

        // valida se cliente e serviço existem
        var clientExists = await dbContext.Clients
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.ClientId, cancellationToken);

        if (!clientExists)
            return ResultDTO<ClientServicePriceDTO>.AsFailure(
                new FailureDTO(400, "Cliente não encontrado."));

        var serviceExists = await dbContext.Services
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.ServiceId, cancellationToken);

        if (!serviceExists)
            return ResultDTO<ClientServicePriceDTO>.AsFailure(
                new FailureDTO(400, "Serviço não encontrado."));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                var actor = context.User.GetUserId();

                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                ClientServicePrice entity = new(
                    request.ClientId,
                    request.ServiceId,
                    request.UnitPrice,
                    request.StartDate,
                    actor);

                await dbContext.ClientServicePrices.AddAsync(entity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dto = new ClientServicePriceDTO(
                    entity.Id,
                    entity.ClientId,
                    entity.ServiceId,
                    entity.UnitPrice,
                    entity.StartDate,
                    entity.EndDate,
                    entity.IsActive);

                return ResultDTO<ClientServicePriceDTO>.AsSuccess(dto);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<ClientServicePriceDTO>.AsFailure(
                    new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<ClientServicePriceDTO>.AsFailure(
                    new FailureDTO(500, "Erro ao cadastrar preço."));
            }
        });

        return result;
    }
}