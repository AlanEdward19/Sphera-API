using Microsoft.EntityFrameworkCore;
using Sphera.API.Auths.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Auths.RefreshToken;

public class RefreshTokenCommandHandle(SpheraDbContext dbContext, ILogger<RefreshTokenCommandHandle> logger, IAuthUtilityService auth)
    : IHandler<RefreshTokenCommand, RefreshTokenDTO>
{
    public async Task<IResultDTO<RefreshTokenDTO>> HandleAsync(RefreshTokenCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.RefreshToken))
            return ResultDTO<RefreshTokenDTO>.AsFailure(new FailureDTO(400, "E-mail e refresh token são obrigatórios."));
        
        var emailFormat = new EmailValueObject(request.Email);

        var user = await dbContext.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == emailFormat, cancellationToken);

        if (user is null)
            return ResultDTO<RefreshTokenDTO>.AsFailure(new FailureDTO(401, "Usuário não encontrado."));

        if (!user.Active)
            return ResultDTO<RefreshTokenDTO>.AsFailure(new FailureDTO(403, "Usuário inativo."));
        
        var newAccessToken = auth.GenerateToken(user);
        var refreshToken = auth.GenerateRefreshToken();

        logger.LogInformation("Refresh token aceito para {Email}. Novo access token emitido.", user.Email.Address);
        return ResultDTO<RefreshTokenDTO>.AsSuccess(new RefreshTokenDTO(newAccessToken, refreshToken));
    }
}