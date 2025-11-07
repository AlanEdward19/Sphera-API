using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users.GeUsers;

public class GetUsersQueryHandler(SpheraDbContext dbContext, ILogger<GetUsersQueryHandler> logger) : IHandler<GetUsersQuery, IEnumerable<UserDTO>>
{
    public async Task<IResultDTO<IEnumerable<UserDTO>>> HandleAsync(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<User> query = dbContext.
            Users
            .AsNoTracking()
            .Include(u => u.Role);
        
        if(string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(u => u.Email.Address == request.Email);
        
        if(request.IsActive.HasValue)
            query = query.Where(u => u.Active == request.IsActive.Value);

        if (request.RoleId.HasValue)
            query = query.Where(u => u.RoleId == request.RoleId.Value);
        
        var users = await query
            .Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : 0))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        return ResultDTO<IEnumerable<UserDTO>>.AsSuccess(users.Select(u => u.ToDTO()));
    }
}