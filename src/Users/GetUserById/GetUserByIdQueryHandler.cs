using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.GetUserById;

public class GetUserByIdQueryHandler(SpheraDbContext dbContext, ILogger<GetUserByIdQueryHandler> logger) : IHandler<GetUserByIdQuery, UserDTO>
{
    public async Task<IResultDTO<UserDTO>> HandleAsync(GetUserByIdQuery request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando recuperação do usuário pelo ID: '{UserId}'", request.Id);
        
        IQueryable<User> query = dbContext.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Contacts);
        
        User? user = await query.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (user is null)
            return ResultDTO<UserDTO>.AsFailure(new FailureDTO(404, "Usuário não encontrado"));
        
        return ResultDTO<UserDTO>.AsSuccess(user.ToDTO());
    }
}