using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Clients.CreateClient;
using Sphera.API.Clients.DTOs;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Users
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateClient([FromServices] IHandler<CreateClientCommand, ClientDTO> handler, [FromBody] CreateClientCommand command,
            CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(command, cancellationToken);

            return response.IsSuccess
                ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
                : BadRequest(response.Failure);
        }
    }
}
