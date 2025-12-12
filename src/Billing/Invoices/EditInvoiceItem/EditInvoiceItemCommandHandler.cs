using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Invoices.EditInvoiceItem;

public class EditInvoiceItemCommandHandler(
    SpheraDbContext dbContext,
    ILogger<EditInvoiceItemCommandHandler> logger)
    : IHandler<EditInvoiceItemCommand, InvoiceDTO>
{
    public async Task<IResultDTO<InvoiceDTO>> HandleAsync(
        EditInvoiceItemCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Editando item {ItemId} da fatura {InvoiceId}", request.GetItemId(), request.GetInvoiceId());

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var invoice = await dbContext.Invoices
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.Id == request.GetInvoiceId(), cancellationToken);

                if (invoice is null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<InvoiceDTO>.AsFailure(new FailureDTO(404, "Fatura não encontrada."));
                }

                var item = invoice.Items.FirstOrDefault(x => x.Id == request.GetItemId());
                if (item is null)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<InvoiceDTO>.AsFailure(new FailureDTO(404, "Item da fatura não encontrado."));
                }

                // Regras de edição
                item.UpdateManualValues(request.Quantity, request.UnitPrice);

                // Recalcular total da fatura
                invoice.RecalculateTotalAmount();

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dto = new InvoiceDTO(
                    invoice.Id,
                    invoice.ClientId,
                    invoice.Name,
                    invoice.IssueDate,
                    invoice.DueDate,
                    invoice.TotalAmount,
                    invoice.Status.ToString(),
                    invoice.Items.Select(i => new InvoiceItemDTO(
                        i.Id,
                        i.ServiceId,
                        i.Description,
                        i.Quantity,
                        i.UnitPrice,
                        i.AdditionalAmount,
                        i.TotalAmount,
                        i.IsAdditional,
                        i.IsManualPriced)).ToList().AsReadOnly());

                return ResultDTO<InvoiceDTO>.AsSuccess(dto);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<InvoiceDTO>.AsFailure(new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<InvoiceDTO>.AsFailure(new FailureDTO(500, "Erro ao editar item da fatura."));
            }
        });

        return result;
    }
}
