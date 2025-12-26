namespace Sphera.API.Billing.Invoices.DTOs;

public record InvoiceDTO(
    Guid Id,
    Guid ClientId,
    string Name,
    DateTime IssueDate,
    DateTime DueDate,
    decimal TotalAmount,
    string Status,
    IReadOnlyCollection<InvoiceItemDTO> Items
);