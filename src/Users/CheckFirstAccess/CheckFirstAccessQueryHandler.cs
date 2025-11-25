using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.CheckFirstAccess;

public class CheckFirstAccessQueryHandler(SpheraDbContext dbContext, ILogger<CheckFirstAccessQueryHandler> logger)
    : IHandler<CheckFirstAccessQuery, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(CheckFirstAccessQuery request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando verificação de primeiro acesso para o email: {Email}", request.Email);
        
        var user = await dbContext
            .Users
            .FindAsync([request.Email], cancellationToken);

        return user is null
            ? ResultDTO<bool>.AsFailure(new FailureDTO(400, "Usuário não encontrado."))
            : ResultDTO<bool>.AsSuccess(user.CheckFirstAccess());
    }
}