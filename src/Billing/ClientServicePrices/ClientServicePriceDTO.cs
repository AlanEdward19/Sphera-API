namespace Sphera.API.Billing.ClientServicePrices;

public record ClientServicePriceDTO(
    Guid Id,
    Guid ClientId,
    Guid ServiceId,
    decimal UnitPrice,
    DateTime StartDate,
    DateTime? EndDate,
    bool IsActive
);