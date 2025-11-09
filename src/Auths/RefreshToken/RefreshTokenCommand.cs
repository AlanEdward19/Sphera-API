namespace Sphera.API.Auths.RefreshToken;

public class RefreshTokenCommand
{
    public string Email { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}