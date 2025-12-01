using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Users.UpdateUser;

public class UpdateUserCommandHandler(SpheraDbContext dbContext, ILogger<UpdateUserCommandHandler> logger) : IHandler<UpdateUserCommand, UserDTO>
{
    public async Task<IResultDTO<UserDTO>> HandleAsync(UpdateUserCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando atualização de usuário {UserId}", request.Id);

        return await ExecutionStrategyHelper.ExecuteAsync(dbContext, async () =>
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
                if (user is null)
                    return ResultDTO<UserDTO>.AsFailure(new FailureDTO(400, "Usuário não encontrado."));

                if (!string.IsNullOrWhiteSpace(request.Name) && !string.Equals(user.Name, request.Name, StringComparison.Ordinal))
                    user.UpdateName(request.Name);

                if (request.RoleId.HasValue && user.RoleId != request.RoleId.Value)
                    user.UpdateRole(request.RoleId.Value);

                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Database.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("Atualização de usuário {UserId} concluída com sucesso", request.Id);
                return ResultDTO<UserDTO>.AsSuccess(user.ToDTO());
            }
            catch (Exception ex)
            {
                await dbContext.Database.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "Erro ao atualizar usuário {UserId}", request.Id);
                return ResultDTO<UserDTO>.AsFailure(new FailureDTO(500, "Erro ao atualizar usuário."));
            }
        }, cancellationToken);
    }
}
