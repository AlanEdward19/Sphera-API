using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.Billing.Invoices.Enums;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Invoices.AddInvoiceAdditionalValue;

public class AddInvoiceAdditionalValueCommandHandler(
    SpheraDbContext dbContext,
    ILogger<AddInvoiceAdditionalValueCommandHandler> logger)
    : IHandler<AddInvoiceAdditionalValueCommand, InvoiceDTO>
{
    public async Task<IResultDTO<InvoiceDTO>> HandleAsync(
        AddInvoiceAdditionalValueCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adicionando valor adicional na fatura {InvoiceId}", request.GetInvoiceId());

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
                    return ResultDTO<InvoiceDTO>.AsFailure(
                        new FailureDTO(404, "Fatura não encontrada."));
                }

                var actor = context.User.GetUserId();

                // TODO se quiser bloquear após envio para Contas a Receber, esse é o ponto.

                //if (invoice.Status == EInvoiceStatus.Closed && invoice.IsSentToReceivables)
                //{
                //    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                //    return ResultDTO<InvoiceDTO>.AsFailure(
                //        new FailureDTO(400, "Fatura já enviada ao Contas a Receber. Não é possível adicionar valores."));
                //}

                invoice.AddAdditionalValue(request.Description, request.Amount);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dto = new InvoiceDTO(
                    invoice.Id,
                    invoice.ClientId,
                    invoice.PeriodStart,
                    invoice.PeriodEnd,
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
                        i.IsAdditional)).ToList().AsReadOnly());

                return ResultDTO<InvoiceDTO>.AsSuccess(dto);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<InvoiceDTO>.AsFailure(
                    new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<InvoiceDTO>.AsFailure(
                    new FailureDTO(500, "Erro ao adicionar valor adicional na fatura."));
            }
        });

        return result;
    }
}