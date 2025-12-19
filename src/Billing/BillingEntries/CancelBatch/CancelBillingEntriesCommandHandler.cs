using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.BillingEntries.Common;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.BillingEntries.CancelBatch;

public class CancelBillingEntriesCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CancelBillingEntriesCommandHandler> logger)
    : IHandler<CancelBillingEntriesCommand, BulkActionResultDTO>
{
    public async Task<IResultDTO<BulkActionResultDTO>> HandleAsync(
        CancelBillingEntriesCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        if (request.Ids.Count == 0)
            return ResultDTO<BulkActionResultDTO>.AsFailure(new FailureDTO(400, "Lista de IDs vazia."));

        var actor = context.User.GetUserId();

        var ids = request.Ids.Distinct().ToList();
        var entities = await dbContext.BillingEntries.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        var foundIds = entities.Select(e => e.Id).ToHashSet();
        var successes = new List<Guid>();

        var failures = ids.Where(id => !foundIds.Contains(id)).Select(missingId => new FailureItemDTO(missingId, "Lançamento não encontrado.")).ToList();

        foreach (var entry in entities)
        {
            try
            {
                entry.Cancel(actor);
                await dbContext.SaveChangesAsync(cancellationToken);
                successes.Add(entry.Id);
            }
            catch (DomainException ex)
            {
                dbContext.Entry(entry).State = EntityState.Unchanged;
                failures.Add(new FailureItemDTO(entry.Id, ex.Message));
            }
            catch (DbUpdateConcurrencyException)
            {
                dbContext.Entry(entry).State = EntityState.Unchanged;
                failures.Add(new FailureItemDTO(entry.Id, "Conflito de concorrência ao atualizar."));
            }
            catch (DbUpdateException)
            {
                dbContext.Entry(entry).State = EntityState.Unchanged;
                failures.Add(new FailureItemDTO(entry.Id, "Erro ao persistir alterações."));
            }
            catch (Exception)
            {
                dbContext.Entry(entry).State = EntityState.Unchanged;
                failures.Add(new FailureItemDTO(entry.Id, "Erro inesperado ao processar."));
            }
        }

        var result = new BulkActionResultDTO(successes, failures);
        if (successes.Count == 0)
            return ResultDTO<BulkActionResultDTO>.AsFailure(new FailureDTO(400, "Nenhum lançamento processado com sucesso."));
        return ResultDTO<BulkActionResultDTO>.AsSuccess(result);
    }
}
