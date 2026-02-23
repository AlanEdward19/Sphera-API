using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Sphera.API.Licensing.DTOs;
using Sphera.API.Licensing.Enums;

namespace Sphera.API.Licensing;

public class LicenseService
{
    private readonly IConfiguration _config;
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly TokenValidationParameters _validationParams;

    public LicenseService(IConfiguration config)
    {
        _config = config;

        var publicKeyPem = File.ReadAllText(config["License:PublicKeyPath"]!);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        
        var rsaParameters = rsa.ExportParameters(false);

        _validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "urn:arqontech:licensing",

            ValidateAudience = true,
            ValidAudience = "urn:arqontech:onpremise",

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsaParameters),

            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };
    }

    private (ELicenseStatus Status, LicenseInfoDTO? Info) Validate()
    {
        var licensePath = _config["License:FilePath"]!;
        if (!File.Exists(licensePath))
            return (ELicenseStatus.Invalid, null);

        try
        {
            var jwt = File.ReadAllText(licensePath);

            var principal = _handler.ValidateToken(jwt, _validationParams, out _);

            var issuedAt = DateTime.Parse(principal.FindFirst("issuedAt")!.Value);
            var expiresAt = DateTime.Parse(principal.FindFirst("expiresAt")!.Value);
            var graceDays = int.Parse(principal.FindFirst("graceDays")!.Value);

            var now = DateTime.UtcNow;

            var status =
                now <= expiresAt
                    ? ELicenseStatus.Active
                    : now <= expiresAt.AddDays(graceDays)
                        ? ELicenseStatus.Grace
                        : ELicenseStatus.Expired;

            var info = new LicenseInfoDTO(
                principal.FindFirst("licenseId")!.Value,
                principal.FindFirst("customerId")!.Value,
                issuedAt,
                expiresAt,
                graceDays
            );

            return (status, info);
        }
        catch
        {
            return (ELicenseStatus.Invalid, null);
        }
    }

    public LicenseCheckResultDTO Check()
    {
        var (status, info) = Validate();
        return new LicenseCheckResultDTO(status, info);
    }
}