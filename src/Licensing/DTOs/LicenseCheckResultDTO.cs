using Sphera.API.Licensing.Enums;

namespace Sphera.API.Licensing.DTOs;

public record LicenseCheckResultDTO(
    ELicenseStatus Status,
    LicenseInfoDTO? Info
);