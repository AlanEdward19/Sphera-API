namespace Sphera.API.Licensing;

public record LicenseInfo(
    string LicenseId,
    string CustomerId,
    DateTime IssuedAt,
    DateTime ExpiresAt,
    int GraceDays
);