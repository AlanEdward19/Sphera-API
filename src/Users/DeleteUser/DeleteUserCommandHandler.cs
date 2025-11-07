using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Users.DeleteUser;

public class DeleteUserCommandHandler(SpheraDbContext dbContext, ILogger<DeleteUserCommandHandler> logger) : IHandler<DeleteUserCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando exclusão de usuário {UserId}", request.Id);
        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await dbContext.Users.FindAsync([request.Id], cancellationToken);
            if (user is null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Usuário não encontrado"));

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException ex)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao deletar usuário."));
        }
    }
}