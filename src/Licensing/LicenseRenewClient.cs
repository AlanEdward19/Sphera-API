namespace Sphera.API.Licensing;

public class LicenseRenewClient(HttpClient http, IConfiguration config)
{
    public async Task<bool> TryRenewAsync(string currentLicense)
    {
        var endpoint = config["License:RenewEndpoint"]!;

        var response = await http.PostAsJsonAsync(endpoint, new
        {
            currentLicense
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadFromJsonAsync<RenewResponse>();
        if (json?.License is null)
            return false;

        await File.WriteAllTextAsync(config["License:FilePath"]!, json.License);
        return true;
    }

    private record RenewResponse(string License, DateTime ExpiresAt);
}