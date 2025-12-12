using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Invoices.ListInvoices;

public class ListInvoicesQueryHandler(
    SpheraDbContext dbContext,
    ILogger<ListInvoicesQueryHandler> logger)
    : IHandler<ListInvoicesQuery, IReadOnlyCollection<InvoiceDTO>>
{
    public async Task<IResultDTO<IReadOnlyCollection<InvoiceDTO>>> HandleAsync(
        ListInvoicesQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Listando faturas");

        IQueryable<Invoice> query = dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Items);

        if (request.ClientId.HasValue)
            query = query.Where(i => i.ClientId == request.ClientId.Value);

        if (request.PeriodStart.HasValue)
            query = query.Where(i => i.DueDate >= request.PeriodStart.Value.Date);

        if (request.PeriodEnd.HasValue)
            query = query.Where(i => i.IssueDate <= request.PeriodEnd.Value.Date);

        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        var invoices = await query
            .OrderBy(i => i.ClientId)
            .ThenBy(i => i.IssueDate)
            .ToListAsync(cancellationToken);

        var dtos = invoices
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
                    i.IsAdditional)).ToList().AsReadOnly()
            ))
            .ToList()
            .AsReadOnly();

        return ResultDTO<IReadOnlyCollection<InvoiceDTO>>.AsSuccess(dtos);
    }
}