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
        var periodStart = request.PeriodStart.Date;
        var periodEnd = request.PeriodEnd.Date;

        if (periodEnd < periodStart)
            return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsFailure(
                new FailureDTO(400, "Período inválido."));

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
                        e.InvoiceId == null &&
                        e.ServiceDate >= periodStart &&
                        e.ServiceDate <= periodEnd);

                if (request.ClientId.HasValue)
                    query = query.Where(e => e.ClientId == request.ClientId.Value);

                var entries = await query.ToListAsync(cancellationToken);

                if (!entries.Any())
                {
                    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                    return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsFailure(
                        new FailureDTO(400, "Nenhum lançamento encontrado para o período."));
                }

                // 2) Agrupar por cliente
                var invoices = new List<Invoice>();

                var groups = entries.GroupBy(e => e.ClientId);

                foreach (var group in groups)
                {
                    var clientId = group.Key;
                    var invoice = new Invoice(clientId, periodStart, periodEnd, actor);

                    foreach (var entry in group)
                    {
                        var price = await GetUnitPriceAsync(clientId, entry.ServiceId, entry.ServiceDate, cancellationToken);

                        if (price == null)
                        {
                            if (request.MissingPriceBehavior == EMissingPriceBehavior.Block)
                                throw new DomainException($"Não há preço configurado para cliente {clientId}, serviço {entry.ServiceId}.");

                            // AllowManual: aplicar 0 e o usuário ajusta depois
                            price = 0m;
                        }

                        var description = entry.Service.Name;
                        invoice.AddItem(entry.ServiceId, description, entry.Quantity, price.Value);

                        entry.AttachToInvoice(invoice.Id);
                    }

                    invoice.Close(actor);
                    invoices.Add(invoice);

                    await dbContext.Invoices.AddAsync(invoice, cancellationToken);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                var dtoList = invoices
                    .Select(inv => new InvoiceDTO(
                        inv.Id,
                        inv.ClientId,
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
                            i.IsAdditional
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
                        p.ServiceId == serviceId &&
                        p.IsValidOn(serviceDate))
            .OrderByDescending(p => p.StartDate)
            .FirstOrDefaultAsync(ct);

        return price?.UnitPrice;
    }
}