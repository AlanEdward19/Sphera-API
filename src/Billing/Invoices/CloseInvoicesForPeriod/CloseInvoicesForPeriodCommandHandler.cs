using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.BillingEntries;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.Billing.Invoices.Enums;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Invoices.CloseInvoicesForPeriod;

public class CloseInvoicesForPeriodCommandHandler(
    SpheraDbContext dbContext,
    ILogger<CloseInvoicesForPeriodCommandHandler> logger)
    : IHandler<CloseInvoicesForPeriodCommand, IReadOnlyCollection<InvoiceDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<InvoiceDTO>>> HandleAsync(
        CloseInvoicesForPeriodCommand request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var actor = context.User.GetUserId();

        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                // 1) Buscar lançamentos faturáveis no período, sem fatura
                IQueryable<BillingEntry> query = dbContext.BillingEntries
                    .Include(e => e.Service)
                    .Where(e =>
                        e.IsBillable &&
                        e.InvoiceId == null);

                if (request.ClientId == Guid.Empty)
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsFailure(
                        new FailureDTO(400, "ClientId é obrigatório."));
                }

                query = query.Where(e => e.ClientId == request.ClientId);

                var entries = await query.ToListAsync(cancellationToken);

                if (!entries.Any())
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsFailure(
                        new FailureDTO(400, "Nenhum lançamento encontrado para o período."));
                }

                // 2) Construir fatura para o cliente informado
                var invoices = new List<Invoice>();

                var clientId = request.ClientId;

                // Definir DueDate conforme cenários
                DateTime invoiceDueDate;
                if (request.Installments.Any())
                {
                    invoiceDueDate = request.Installments.Max(i => i.DueDate).Date;
                }
                else
                {
                    invoiceDueDate = request.DueDate.Value.Date;
                }

                var invoice = new Invoice(clientId, request.IssueDate, invoiceDueDate, actor);

                foreach (var entry in entries)
                {
                    var originalPrice =
                        await GetUnitPriceAsync(clientId, entry.ServiceId, entry.ServiceDate, cancellationToken);
                    var isManual = false;
                    decimal price;
                    if (originalPrice == null)
                    {
                        if (request.MissingPriceBehavior == EMissingPriceBehavior.Block)
                            throw new DomainException(
                                $"Não há preço configurado para cliente {clientId}, serviço {entry.ServiceId}.");
                        // AllowManual: aplicar 0 e o usuário ajusta depois
                        price = 0m;
                        isManual = true;
                    }
                    else price = originalPrice.Value;

                    var description = entry.Service.Name;
                    invoice.AddItem(entry.ServiceId, description, entry.Quantity, price, isManual);

                    entry.AttachToInvoice(invoice.Id);
                }

                // Ajuste opcional para TotalAmount (cenário 1)
                if (request.TotalAmount.HasValue)
                {
                    var diff = request.TotalAmount.Value - invoice.TotalAmount;
                    if (diff != 0)
                    {
                        var desc = diff > 0 ? "Ajuste de fechamento (acréscimo)" : "Ajuste de fechamento (desconto)";
                        invoice.AddAdditionalValue(desc, diff);
                    }
                }

                invoice.Close(actor);

                // Gerar parcelas conforme cenários
                if (request.Installments.Any())
                {
                    invoice.SetInstallments(request.Installments
                        .Select(i => (i.Number, i.Amount, i.DueDate)));
                }
                else if (request.TotalAmount.HasValue && request.DueDate.HasValue)
                {
                    invoice.SetInstallments(new[] { (1, request.TotalAmount.Value, request.DueDate.Value.Date) });
                }

                invoices.Add(invoice);
                await dbContext.Invoices.AddAsync(invoice, cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dtoList = invoices
                    .Select(inv => new InvoiceDTO(
                        inv.Id,
                        inv.ClientId,
                        inv.Name,
                        inv.IssueDate,
                        inv.DueDate,
                        inv.TotalAmount,
                        inv.Status.ToString(),
                        inv.Items.Select(i => new InvoiceItemDTO(
                            i.Id,
                            i.ServiceId,
                            i.Description,
                            i.Quantity,
                            i.UnitPrice,
                            i.AdditionalAmount,
                            i.TotalAmount,
                            i.IsAdditional,
                            i.IsManualPriced
                        )).ToList().AsReadOnly()
                    ))
                    .ToList()
                    .AsReadOnly();

                return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsSuccess(dtoList);
            }
            catch (DomainException ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsFailure(
                    new FailureDTO(400, ex.Message));
            }
            catch (Exception)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsFailure(
                    new FailureDTO(500, "Erro ao fechar faturas."));
            }
        });

        return result;
    }

    private async Task<decimal?> GetUnitPriceAsync(
        Guid clientId,
        Guid serviceId,
        DateTime serviceDate,
        CancellationToken ct)
    {
        var price = await dbContext.ClientServicePrices
            .Where(p => p.ClientId == clientId &&
                        p.ServiceId == serviceId && (p.IsActive &&
                                                     serviceDate.Date >= p.StartDate.Date &&
                                                     (p.EndDate == null || serviceDate.Date <= p.EndDate.Value.Date)))
            .OrderByDescending(p => p.StartDate)
            .FirstOrDefaultAsync(ct);

        return price?.UnitPrice;
    }
}