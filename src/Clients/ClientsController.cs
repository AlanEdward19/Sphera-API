using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Clients.ActivateClient;
using Sphera.API.Clients.CreateClient;
using Sphera.API.Clients.DeactivateClient;
using Sphera.API.Clients.DeleteClient;
using Sphera.API.Clients.DTOs;
using Sphera.API.Clients.GetClientById;
using Sphera.API.Clients.GetClients;
using Sphera.API.Clients.UpdateClient;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Clients;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    [HttpGet(Name = "GetClients")]
    public async Task<IActionResult> GetClients([FromServices] IHandler<GetClientsQuery, IEnumerable<ClientDTO>> handler,
        [FromQuery] GetClientsQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetClientById")]
    public async Task<IActionResult> GetClientById([FromServices] IHandler<GetClientByIdQuery, ClientDTO> handler, Guid id,
        CancellationToken cancellationToken, [FromQuery] bool includePartner = false)
    {
        var query = new GetClientByIdQuery(id, includePartner);

        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPost(Name = "CreateClient")]
    public async Task<IActionResult> CreateClient([FromServices] IHandler<CreateClientCommand, ClientDTO> handler, [FromBody] CreateClientCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPut("{id:guid}", Name = "UpdateClient")]
    public async Task<IActionResult> UpdateClient([FromServices] IHandler<UpdateClientCommand, ClientDTO> handler, [FromBody] UpdateClientCommand command,
        Guid id, CancellationToken cancellationToken)
    {
        command.SetId(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
           ? Ok(response.Success)
           : BadRequest(response.Failure);
    }

    [HttpPatch("{id:guid}/activate", Name = "ActivateClient")]
    public async Task<IActionResult> ActivateClient([FromServices] IHandler<ActivateClientCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateClientCommand(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }

    [HttpPatch("{id:guid}/deactivate", Name = "DeactivateClient")]
    public async Task<IActionResult> DeactivateClient([FromServices] IHandler<DeactivateClientCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateClientCommand(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }

    [HttpDelete("{id:guid}", Name = "DeleteClient")]
    public async Task<IActionResult> DeleteClient([FromServices] IHandler<DeleteClientCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteClientCommand(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }
}