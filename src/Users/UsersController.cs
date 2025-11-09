using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;
using Sphera.API.Users.GetUsers;
using Sphera.API.Users.UpdateUser;
using Sphera.API.Users.ActivateUser;
using Sphera.API.Users.ChangePassword;
using Sphera.API.Users.DeactivateUser;
using Sphera.API.Users.DeleteUser;
using Sphera.API.Users.FirstAccessPassword;

namespace Sphera.API.Users
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "GetUsers")]
        public async Task<IActionResult> GetUsers([FromServices] IHandler<GetUsersQuery, IEnumerable<UserDTO>> handler, [FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Success)
                : BadRequest(response.Failure);
        }
        
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateClient([FromServices] IHandler<CreateUserCommand, UserDTO> handler, [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var user = HttpContext.User;
            
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

            return response.IsSuccess
                ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
                : BadRequest(response.Failure);
        }
        
        [HttpPut("{id:guid}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromServices] IHandler<UpdateUserCommand, UserDTO> handler, [FromBody] UpdateUserCommand command,
            Guid id, CancellationToken cancellationToken)
        {
            command.SetId(id);
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Success)
                : BadRequest(response.Failure);
        }
        
        [HttpPatch("{id:guid}/activate", Name = "ActivateUser")]
        public async Task<IActionResult> ActivateUser([FromServices] IHandler<ActivateUserCommand, bool> handler, Guid id, CancellationToken cancellationToken)
        {
            var command = new ActivateUserCommand(id);
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

            return response.IsSuccess
                ? NoContent()
                : BadRequest(response.Failure);
        }

        [HttpPatch("{id:guid}/deactivate", Name = "DeactivateUser")]
        public async Task<IActionResult> DeactivateUser([FromServices] IHandler<DeactivateUserCommand, bool> handler, Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateUserCommand(id);
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

            return response.IsSuccess
                ? NoContent()
                : BadRequest(response.Failure);
        }

        [HttpDelete("{id:guid}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromServices] IHandler<DeleteUserCommand, bool> handler, Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand(id);
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

            return response.IsSuccess
                ? NoContent()
                : BadRequest(response.Failure);
        }
        
        [HttpPatch("{id:guid}/change-password", Name = "ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromServices] IHandler<ChangePasswordCommand, bool> handler, [FromBody] ChangePasswordCommand command, Guid id, CancellationToken cancellationToken)
        {
            command.SetId(id);
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
            return response.IsSuccess 
                ? NoContent() 
                : BadRequest(response.Failure);
        }
        
        [HttpPatch("{id:guid}/first-password", Name = "FirstAccessPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> FirstAccessPassword([FromServices] IHandler<FirstAccessPasswordCommand, bool> handler, [FromBody] FirstAccessPasswordCommand command, Guid id, CancellationToken cancellationToken)
        {
            command.SetId(id);
            var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
            return response.IsSuccess 
                ? NoContent() 
                : BadRequest(response.Failure);
        }
    }
}
