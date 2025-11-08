using System.Security.Claims;

namespace Sphera.API.Shared.Utils;

public static class ClaimsPrincipalUtils
{
    public static string GetEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst("emails");
        return emailClaim?.Value ?? string.Empty;
    }

    public static string GetUserId(this ClaimsPrincipal user)
        => user.FindFirst("oid")?.Value ?? string.Empty;
    
    public static string GetName(this ClaimsPrincipal user)
        => user.FindFirst("name")?.Value ?? string.Empty;
}