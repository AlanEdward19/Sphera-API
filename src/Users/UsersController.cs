using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;
using Sphera.API.Users.GetUsers;
using Sphera.API.Users.UpdateUser;

namespace Sphera.API.Users
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "GetUsers")]
        public async Task<IActionResult> GetUsers([FromServices] IHandler<GetUsersQuery, IEnumerable<UserDTO>> handler, [FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(query, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Success)
                : BadRequest(response.Failure);
        }
        
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateClient([FromServices] IHandler<CreateUserCommand, UserDTO> handler, [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(command, cancellationToken);

            return response.IsSuccess
                ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
                : BadRequest(response.Failure);
        }
        
        [HttpPut("{id:guid}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromServices] IHandler<UpdateUserCommand, UserDTO> handler, [FromBody] UpdateUserCommand command,
            Guid id, CancellationToken cancellationToken)
        {
            command.SetId(id);
            var response = await handler.HandleAsync(command, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Success)
                : BadRequest(response.Failure);
        }
    }
}
