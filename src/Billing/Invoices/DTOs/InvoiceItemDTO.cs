namespace Sphera.API.Billing.Invoices.DTOs;

public record InvoiceItemDTO(
    Guid Id,
    Guid? ServiceId,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal AdditionalAmount,
    decimal TotalAmount,
    bool IsAdditional,
    bool IsManualPriced
);