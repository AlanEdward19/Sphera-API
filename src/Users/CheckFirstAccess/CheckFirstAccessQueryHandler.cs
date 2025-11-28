using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.ValueObjects;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.CheckFirstAccess;

public class CheckFirstAccessQueryHandler(SpheraDbContext dbContext, ILogger<CheckFirstAccessQueryHandler> logger)
    : IHandler<CheckFirstAccessQuery, FirstAccessUserDTO>
{
    public async Task<IResultDTO<FirstAccessUserDTO>> HandleAsync(CheckFirstAccessQuery request, HttpContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando verificação de primeiro acesso para o email: {Email}", request.Email);
        
        var emailValueObject = new EmailValueObject(request.Email);
        
        var user = await dbContext
            .Users
            .FirstOrDefaultAsync(u => u.Email.Equals(emailValueObject), cancellationToken);

        return user is null
            ? ResultDTO<FirstAccessUserDTO>.AsFailure(new FailureDTO(400, "Usuário não encontrado."))
            : ResultDTO<FirstAccessUserDTO>.AsSuccess(new FirstAccessUserDTO { IsFirstAccess = user.CheckFirstAccess(), Id = user.Id } );
    }
}