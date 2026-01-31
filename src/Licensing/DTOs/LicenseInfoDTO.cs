namespace Sphera.API.Licensing.DTOs;

public record LicenseInfoDTO(
    string LicenseId,
    string CustomerId,
    DateTime IssuedAt,
    DateTime ExpiresAt,
    int GraceDays
);