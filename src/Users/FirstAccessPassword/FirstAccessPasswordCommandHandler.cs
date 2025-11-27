using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Users.FirstAccessPassword;

public class FirstAccessPasswordCommandHandler(SpheraDbContext dbContext, ILogger<FirstAccessPasswordCommandHandler> logger, IAuthUtilityService auth)
    : IHandler<FirstAccessPasswordCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(FirstAccessPasswordCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando definição de senha de primeiro acesso para usuário {UserId}", request.GetId());

        await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.GetId(), cancellationToken);
            if (user is null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Usuário não encontrado."));

            if (!user.IsFirstAccess)
                return ResultDTO<bool>.AsFailure(new FailureDTO(409,
                    "Senha de primeiro acesso já foi definida para este usuário."));

            user.ChangePassword(request.NewPassword);
            user.PasswordHash(auth.HashPassword(request.NewPassword));
            dbContext.Users.Update(user);

            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Senha de primeiro acesso definida com sucesso para usuário {UserId}",
                request.GetId());
            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "Erro ao definir senha de primeiro acesso do usuário {UserId}", request.GetId());
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao definir senha de primeiro acesso do usuário."));
        }
    }
}
