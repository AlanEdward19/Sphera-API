using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BillingEntries.UpdateBillingEntry;

public class UpdateBillingEntryCommandHandler(
    SpheraDbContext dbContext,
    ILogger<UpdateBillingEntryCommandHandler> logger)
    : IHandler<UpdateBillingEntryCommand, BillingEntryDTO>
{
    public async Task<IResultDTO<BillingEntryDTO>> HandleAsync(
        UpdateBillingEntryCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Atualizando BillingEntry {Id}", request.GetId());

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = await dbContext.BillingEntries
                    .FirstOrDefaultAsync(x => x.Id == request.GetId(), cancellationToken);

                if (entity is null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<BillingEntryDTO>.AsFailure(
                        new FailureDTO(404, "Lançamento não encontrado."));
                }

                if (entity.InvoiceId != null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<BillingEntryDTO>.AsFailure(
                        new FailureDTO(400, "Não é possível alterar lançamento já faturado."));
                }

                var actor = context.User.GetUserId();

                entity.UpdateQuantity(request.Quantity, actor);
                entity.SetBillable(request.IsBillable, actor);

                if (!string.IsNullOrWhiteSpace(request.Notes))
                    entity.SetNotes(request.Notes, actor);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dto = new BillingEntryDTO(
                    entity.Id,
                    entity.ClientId,
                    entity.ServiceId,
                    entity.ServiceDate,
                    entity.Quantity,
                    entity.IsBillable,
                    entity.Notes,
                    entity.InvoiceId);

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
                    new FailureDTO(500, "Erro ao atualizar lançamento."));
            }
        });

        return result;
    }
}