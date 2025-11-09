using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Services.ActivateService;
using Sphera.API.Services.CreateService;
using Sphera.API.Services.DeactivateService;
using Sphera.API.Services.DeleteService;
using Sphera.API.Services.GetServiceById;
using Sphera.API.Services.GetServices;
using Sphera.API.Services.UpdateService;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Services;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    [HttpGet(Name = "GetServices")]
    public async Task<IActionResult> GetServices([FromServices] IHandler<GetServicesQuery, IEnumerable<ServiceDTO>> handler,
        [FromQuery] GetServicesQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetServiceById")]
    public async Task<IActionResult> GetServiceById([FromServices] IHandler<GetServiceByIdQuery, ServiceDTO> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetServiceByIdQuery(id);
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPost(Name = "CreateService")]
    public async Task<IActionResult> CreateService([FromServices] IHandler<CreateServiceCommand, ServiceDTO> handler, [FromBody] CreateServiceCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? CreatedAtRoute("GetServiceById", new { id = response.Success!.Id }, response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPut("{id:guid}", Name = "UpdateService")]
    public async Task<IActionResult> UpdateService([FromServices] IHandler<UpdateServiceCommand, ServiceDTO> handler,
        [FromBody] UpdateServiceCommand command, Guid id, CancellationToken cancellationToken)
    {
        command.SetId(id);

        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPatch("{id:guid}/activate", Name = "ActivateService")]
    public async Task<IActionResult> ActivateService([FromServices] IHandler<ActivateServiceCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateServiceCommand(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }

    [HttpPatch("{id:guid}/deactivate", Name = "DeactivateService")]
    public async Task<IActionResult> DeactivateService([FromServices] IHandler<DeactivateServiceCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateServiceCommand(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }

    [HttpDelete("{id:guid}", Name = "DeleteService")]
    public async Task<IActionResult> DeleteService([FromServices] IHandler<DeleteServiceCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteServiceCommand(id);

        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }
}