using Sphera.API.Users;

namespace Sphera.API.Shared.Interfaces;

public interface IAuthUtilityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerateToken(User user);
    string GenerateRefreshToken();
}