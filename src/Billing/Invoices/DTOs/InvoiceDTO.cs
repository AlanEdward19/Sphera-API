namespace Sphera.API.Billing.Invoices.DTOs;

public record InvoiceDTO(
    Guid Id,
    Guid ClientId,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal TotalAmount,
    string Status,
    IReadOnlyCollection<InvoiceItemDTO> Items
);