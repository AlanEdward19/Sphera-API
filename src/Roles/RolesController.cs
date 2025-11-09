using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Roles.DTOs;
using Sphera.API.Roles.GetRoles;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Roles;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    [HttpGet(Name = "GetRoles")]
    public async Task<IActionResult> GetRoles([FromServices] IHandler<GetRolesQuery, IEnumerable<RoleDTO>> handler,
        [FromQuery] GetRolesQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }
}