using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Auths.DTOs;

public class LoginDTO
{
    public LoginDTO(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }
    public string Token { get; private set; }
    public string RefreshToken { get; private set; }
}