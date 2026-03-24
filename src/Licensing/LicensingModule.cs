namespace Sphera.API.Licensing;

public static class LicensingModule
{
    public static IServiceCollection ConfigureLicensingModule(this IServiceCollection services)
    {
        services.AddSingleton<LicenseService>();

        services.AddHttpClient<LicenseRenewClient>();
        services.AddSingleton<LicenseRenewClient>();

        return services;
    }
}