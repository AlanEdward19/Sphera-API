namespace Sphera.API.Auths.DTOs;

public class RefreshTokenDTO(string token, string refreshToken) : LoginDTO(token, refreshToken);