using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateClient([FromServices] IHandler<CreateUserCommand, UserDTO> handler, [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(command, cancellationToken);

            return response.IsSuccess
                ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
                : BadRequest(response.Failure);
        }
    }
}
