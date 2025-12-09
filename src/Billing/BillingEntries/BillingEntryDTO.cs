namespace Sphera.API.Billing.BillingEntries;

public record BillingEntryDTO(
    Guid Id,
    Guid ClientId,
    Guid ServiceId,
    DateTime ServiceDate,
    decimal Quantity,
    bool IsBillable,
    string? Notes,
    Guid? InvoiceId
);