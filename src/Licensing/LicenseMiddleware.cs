using System.Net;
using System.Text.Json;
using Sphera.API.Licensing.Enums;

namespace Sphera.API.Licensing;

public class LicenseMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        LicenseService licenseService,
        LicenseRenewClient renewClient,
        IConfiguration configuration)
    {
        var check = licenseService.Check();

        if (check.Status == ELicenseStatus.Active)
        {
            await next(context);
            return;
        }
        
        var licensePath = configuration["License:FilePath"]!;
        
        if (check is { Status: ELicenseStatus.Grace, Info: not null })
        {
            _ = Task.Run(() =>
                renewClient.TryRenewAsync(
                    File.ReadAllText(licensePath)
                ));
            
            int daysLeft = check.Info.ExpiresAt.AddDays(check.Info.GraceDays)
                .Subtract(DateTime.UtcNow)
                .Days;
            
            var formattedAmount = daysLeft.ToString();
            
            context.Response.Headers["X-License-Alert"] = formattedAmount;
        
            await next(context);
            return;
        }

        if (check is { Status: ELicenseStatus.Expired, Info: not null })
        {
            var renewed = await renewClient.TryRenewAsync(
                await File.ReadAllTextAsync(licensePath)
            );

            if (renewed)
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json; charset=utf-8";
            
            context.Response.Headers["X-License-Alert"] = "0";

            var payload = new { code = 403, message = "Licença expirada" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Response.ContentType = "application/json; charset=utf-8";
        
        context.Response.Headers["X-License-Alert"] = "0";

        var invalidPayload = new { code = 403, message = "Licença inválida" };
        await context.Response.WriteAsync(JsonSerializer.Serialize(invalidPayload));
    }
}