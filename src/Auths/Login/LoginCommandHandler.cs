using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Services;
using Sphera.API.Users;
using Sphera.API.Auths.DTOs;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Auths.Login;

public class LoginCommandHandler(SpheraDbContext dbContext, ILogger<LoginCommandHandler> logger, IAuthUtilityService auth) : IHandler<LoginCommand, LoginDTO>
{
    public async Task<IResultDTO<LoginDTO>> HandleAsync(LoginCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return ResultDTO<LoginDTO>.AsFailure(new FailureDTO(400, "E-mail e senha são obrigatórios."));
        
        var emailFormat = new EmailValueObject(request.Email);

        var user = await dbContext
            .Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == emailFormat, cancellationToken);

        if (user is null)
            return ResultDTO<LoginDTO>.AsFailure(new FailureDTO(401, "Credenciais inválidas."));

        if (!user.Active)
            return ResultDTO<LoginDTO>.AsFailure(new FailureDTO(403, "Usuário inativo."));
        
        var isValidPassword = auth.VerifyPassword(request.Password, user.Password.Value);
        if (!isValidPassword)
            return ResultDTO<LoginDTO>.AsFailure(new FailureDTO(401, "Credenciais inválidas."));

        var token = auth.GenerateToken(user);
        var refreshToken = auth.GenerateRefreshToken();

        logger.LogInformation("Usuário {Email} autenticado com sucesso", user.Email.Address);
        return ResultDTO<LoginDTO>.AsSuccess(new LoginDTO(token, refreshToken));
    }
}