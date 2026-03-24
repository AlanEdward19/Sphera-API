using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Users.ActivateUser;

public class ActivateUserCommandHandler(SpheraDbContext dbContext, ILogger<ActivateUserCommandHandler> logger)
    : IHandler<ActivateUserCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(ActivateUserCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Definindo status do Usuário: '{UserId}' para ativado.", request.Id);

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var user = await dbContext.Users.FindAsync([request.Id], cancellationToken);

                if (user is null)
                    return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Usuário não encontrado"));

                user.Activate();
                dbContext.Users.Update(user);

                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                return ResultDTO<bool>.AsSuccess(true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Um erro ocorreu ao tentar definir o status do usuário para ativo.");
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Um erro ocorreu ao tentar definir o status do usuário para ativo."));
            }
        }, cancellationToken);
    }
}