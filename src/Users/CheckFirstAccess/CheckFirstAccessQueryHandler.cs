using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.CheckFirstAccess;

public class CheckFirstAccessQueryHandler(SpheraDbContext dbContext, ILogger<CheckFirstAccessQueryHandler> logger)
    : IHandler<CheckFirstAccessQuery, UserDTO>
{
    public async Task<IResultDTO<UserDTO>> HandleAsync(CheckFirstAccessQuery request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando verificação de primeiro acesso para o email: {Email}", request.Email);
        
        var user = await dbContext
            .Users
            .FindAsync([request.Email], cancellationToken);

        return user is null
            ? ResultDTO<UserDTO>.AsFailure(new FailureDTO(400, "Usuário não encontrado."))
            : ResultDTO<UserDTO>.AsSuccess(user.ToDTO());
    }
}