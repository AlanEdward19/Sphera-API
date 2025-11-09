using System.Security.Claims;

namespace Sphera.API.Shared.Utils;

public static class ClaimsPrincipalUtils
{
    public static string GetEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst("emails");
        return emailClaim?.Value ?? string.Empty;
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.Actor)?.Value;
        return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
    }

    public static string GetName(this ClaimsPrincipal user)
        => user.FindFirst("name")?.Value ?? string.Empty;
}