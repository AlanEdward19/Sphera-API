using System.Net;
using Sphera.API.Licensing.Enums;

namespace Sphera.API.Licensing;

public class LicenseMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        LicenseService licenseService, 
        LicenseRenewClient renewClient)
    {
        var check = licenseService.Check();
        
        if (check.Status == ELicenseStatus.Active)
        {
            await next(context);
            return;
        }
        
        if (check.Status == ELicenseStatus.Grace && check.Info is not null)
        {
            _ = Task.Run(() =>
                renewClient.TryRenewAsync(
                    File.ReadAllText("license.jwt")
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
                await File.ReadAllTextAsync("license.jwt")
            );

            if (renewed)
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync("Licença expirada");
            return;
        }
        
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        await context.Response.WriteAsync("Licença inválida");
    }
}