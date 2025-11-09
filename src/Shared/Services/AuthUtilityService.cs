using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users;

namespace Sphera.API.Shared.Services;

public class AuthUtilityService(IConfiguration configuration) : IAuthUtilityService
{
    public string HashPassword(string password)
    {
        using var has = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = has.ComputeHash(passwordBytes);
        var builder = new StringBuilder();
        foreach (var hashByte in hashBytes)
            builder.Append(hashByte.ToString("x2"));
        return builder.ToString();
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;
        var computed = HashPassword(password);
        return string.Equals(computed, hash, StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateToken(User user)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim("emails", user.Email.Address),
            new Claim("name", user.Name),
            new Claim(ClaimTypes.Actor, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };
        
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncoder.Encode(bytes);
    }
}