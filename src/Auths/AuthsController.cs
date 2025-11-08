using Microsoft.AspNetCore.Mvc;
using Sphera.API.Auths.DTOs;
using Sphera.API.Auths.Login;
using Sphera.API.Auths.RefreshToken;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Auths;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthsController : ControllerBase
{
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> Login([FromServices] IHandler<LoginCommand, LoginDTO> handler, [FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : BadRequest(response.Failure);
    }

    [HttpPost("refresh", Name = "RefreshToken")]
    public async Task<IActionResult> Refresh([FromServices] IHandler<RefreshTokenCommand, RefreshTokenDTO> handler, [FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : BadRequest(response.Failure);
    }
}