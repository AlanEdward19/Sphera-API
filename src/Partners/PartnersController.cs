using Azure;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Partners.CreatePartner;
using Sphera.API.Partners.DeletePartner;
using Sphera.API.Partners.DTOs;
using Sphera.API.Partners.GetPartnerById;
using Sphera.API.Partners.GetPartners;
using Sphera.API.Partners.UpdatePartner;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Partners;

[ApiController]
[Route("api/v1/[controller]")]
public class PartnersController : ControllerBase
{
    [HttpGet(Name = "GetPartners")]
    public async Task<IActionResult> GetPartners([FromServices] IHandler<GetPartnersQuery, IEnumerable<PartnerDTO>> handler,
        [FromQuery] GetPartnersQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetPartnerById")]
    public async Task<IActionResult> GetPartnerById([FromServices] IHandler<GetPartnerByIdQuery, PartnerDTO> handler, Guid id, CancellationToken cancellationToken, [FromQuery] bool includeClients = false)
    {
        var query = new GetPartnerByIdQuery(id, includeClients);

        var response = await handler.HandleAsync(query, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPost(Name = "CreatePartner")]
    public async Task<IActionResult> CreatePartner([FromServices] IHandler<CreatePartnerCommand, PartnerDTO> handler,
        [FromBody] CreatePartnerCommand command, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, cancellationToken);

        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPut("{id:guid}", Name = "UpdatePartner")]
    public async Task<IActionResult> UpdatePartner([FromServices] IHandler<UpdatePartnerCommand, PartnerDTO> handler, [FromBody] UpdatePartnerCommand command,
        Guid id, CancellationToken cancellationToken)
    {
        command.SetId(id);
        var response = await handler.HandleAsync(command, cancellationToken);

        return response.IsSuccess
           ? Ok(response.Success)
           : BadRequest(response.Failure);
    }

    [HttpPatch("{id:guid}/activate", Name = "ActivatePartner")]
    public async Task<IActionResult> ActivatePartner(Guid id)
    {
        return Ok();
    }

    [HttpPatch("{id:guid}/deactivate", Name = "DeactivatePartner")]
    public async Task<IActionResult> DeactivatePartner(Guid id)
    {
        return Ok();
    }

    [HttpDelete("{id:guid}", Name = "DeletePartner")]
    public async Task<IActionResult> DeletePartner([FromServices] IHandler<DeletePartnerCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePartnerCommand(id);

        var response = await handler.HandleAsync(command, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }
}