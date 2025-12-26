using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BillingEntries.CreateBillingEntry;

public class CreateBillingEntryCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateBillingEntryCommandHandler> logger)
    : IHandler<CreateBillingEntryCommand, BillingEntryDTO>
{
    public async Task<IResultDTO<BillingEntryDTO>> HandleAsync(
        CreateBillingEntryCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando lançamento para cliente {ClientId}, serviço {ServiceId}",
            request.ClientId, request.ServiceId);

        var clientExists = await dbContext.Clients
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.ClientId, cancellationToken);

        if (!clientExists)
            return ResultDTO<BillingEntryDTO>.AsFailure(
                new FailureDTO(400, "Cliente não encontrado."));

        var serviceExists = await dbContext.Services
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.ServiceId, cancellationToken);

        if (!serviceExists)
            return ResultDTO<BillingEntryDTO>.AsFailure(
                new FailureDTO(400, "Serviço não encontrado."));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                var actor = context.User.GetUserId();

                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                BillingEntry entry = new(
                    request.ClientId,
                    request.ServiceId,
                    request.Quantity,
                    request.ServiceDate,
                    request.Notes,
                    actor);

                await dbContext.BillingEntries.AddAsync(entry, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dto = new BillingEntryDTO(
                    entry.Id,
                    entry.ClientId,
                    entry.ServiceId,
                    entry.ServiceDate,
                    entry.Quantity,
                    entry.IsBillable,
                    entry.Notes,
                    entry.InvoiceId);

                return ResultDTO<BillingEntryDTO>.AsSuccess(dto);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<BillingEntryDTO>.AsFailure(
                    new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<BillingEntryDTO>.AsFailure(
                    new FailureDTO(500, "Erro ao criar lançamento."));
            }
        });

        return result;
    }
}