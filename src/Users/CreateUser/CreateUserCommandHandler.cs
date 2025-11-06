using Sphera.API.Clients.CreateClient;
using Sphera.API.Clients.DTOs;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.CreateUser;

public class CreateUserCommandHandler(SpheraDbContext dbContext, ILogger<CreateUserCommandHandler> logger) : IHandler<CreateUserCommand, UserDTO>
{
    public Task<IResultDTO<UserDTO>> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}