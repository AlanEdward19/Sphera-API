using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Users.ChangePassword;

public class ChangePasswordCommandHandler(SpheraDbContext dbContext, ILogger<ChangePasswordCommandHandler> logger, IAuthUtilityService auth)
    : IHandler<ChangePasswordCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(ChangePasswordCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando alteração de senha para usuário {UserId}", request.Id);

        await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user is null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Usuário não encontrado."));
            
            user.ChangePassword(request.NewPassword);
            user.PasswordHash(auth.HashPassword(request.NewPassword));
            dbContext.Users.Update(user);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Senha alterada com sucesso para usuário {UserId}", request.Id);
            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (Exception ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "Erro ao alterar senha do usuário {UserId}", request.Id);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao alterar senha do usuário."));
        }
    }
}