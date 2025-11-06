using Sphera.API.Clients.CreateClient;
using Sphera.API.Clients.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.CreateUser;

public class CreateUserCommandHandler(SpheraDbContext dbContext, ILogger<CreateUserCommandHandler> logger) : IHandler<CreateUserCommand, UserDTO>
{
    public async Task<IResultDTO<UserDTO>> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação de usuário {Email}", request.Email);
        
        await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if(await dbContext.Roles.FindAsync( [request.RoleId], cancellationToken) is null)
                return ResultDTO<UserDTO>.AsFailure(new FailureDTO(400, "Função não encontrada."));
            
            User user = new(request);
            
            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Database.CommitTransactionAsync(cancellationToken);
            
            return ResultDTO<UserDTO>.AsSuccess(user.ToDTO());
        }
        catch (Exception)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
            return ResultDTO<UserDTO>.AsFailure(new FailureDTO(500, "Erro ao criar usuário."));
        }
    }
}