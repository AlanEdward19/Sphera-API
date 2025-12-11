using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Invoices.GetInvoiceById;

public class GetInvoiceByIdQueryHandler(
    SpheraDbContext dbContext,
    ILogger<GetInvoiceByIdQueryHandler> logger)
    : IHandler<GetInvoiceByIdQuery, InvoiceDTO>
{
    public async Task<IResultDTO<InvoiceDTO>> HandleAsync(
        GetInvoiceByIdQuery request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando fatura {Id}", request.Id);

        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice is null)
            return ResultDTO<InvoiceDTO>.AsFailure(
                new FailureDTO(404, "Fatura não encontrada."));

        var dto = new InvoiceDTO(
            invoice.Id,
            invoice.ClientId,
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
                i.IsAdditional)).ToList().AsReadOnly());

        return ResultDTO<InvoiceDTO>.AsSuccess(dto);
    }
}