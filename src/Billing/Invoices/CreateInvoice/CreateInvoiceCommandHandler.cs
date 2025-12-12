using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Invoices.CreateInvoice;

public class CreateInvoiceCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CreateInvoiceCommandHandler> logger)
    : IHandler<CreateInvoiceCommand, InvoiceDTO>
{
    public async Task<IResultDTO<InvoiceDTO>> HandleAsync(
        CreateInvoiceCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Criando fatura para cliente {ClientId}", request.ClientId);

        var clientExists = await dbContext.Clients
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.ClientId, cancellationToken);

        if (!clientExists)
            return ResultDTO<InvoiceDTO>.AsFailure(
                new FailureDTO(400, "Cliente não encontrado."));

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                var actor = context.User.GetUserId();
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                // Construção da fatura
                var issue = request.IssueDate.Date;
                var due = request.DueDate.Date;

                var invoice = new Invoice(request.ClientId, issue, due, actor);
                invoice.SetName(request.Name);

                // Itens
                foreach (var item in request.Items)
                {
                    if (item.IsAdditional)
                    {
                        var amount = item.AdditionalAmount > 0 ? item.AdditionalAmount : item.UnitPrice;
                        invoice.AddAdditionalValue(item.Description, amount);
                    }
                    else
                    {
                        if (!item.ServiceId.HasValue || item.ServiceId.Value == Guid.Empty)
                            throw new DomainException("ServiceId obrigatório para item de serviço.");

                        invoice.AddItem(item.ServiceId.Value, item.Description, item.Quantity, item.UnitPrice);
                    }
                }

                // Parcelas (opcional, mas se vier precisa validar com DueDate)
                if (request.Installments.Any())
                {
                    invoice.SetInstallments(request.Installments
                        .Select(i => (i.Number, i.Amount, i.DueDate)));
                }

                await dbContext.Invoices.AddAsync(invoice, cancellationToken);
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
                        i.IsAdditional
                    )).ToList().AsReadOnly());

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
                return ResultDTO<InvoiceDTO>.AsFailure(new FailureDTO(500, "Erro ao criar fatura."));
            }
        });

        return result;
    }
}
