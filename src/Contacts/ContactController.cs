using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Contacts.AddContactToClient;
using Sphera.API.Contacts.AddContactToPartner;
using Sphera.API.Contacts.EditContact;
using Sphera.API.Contacts.RemoveContactFromClient;
using Sphera.API.Contacts.RemoveContactFromPartner;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Contacts;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ContactController : ControllerBase
{
    [HttpPost("/api/v1/Clients/{clientId:guid}/Contacts", Name = "AddContactToClient")]
    public async Task<IActionResult> AddContactToClient([FromServices] IHandler<AddContactToClientCommand, ContactDTO> handler,
        [FromBody] AddContactToClientCommand command, Guid clientId, CancellationToken cancellationToken)
    {
        command.SetClientId(clientId);

        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPost("/api/v1/Partners/{partnerId:guid}/Contacts", Name = "AddContactToPartner")]
    public async Task<IActionResult> AddContactToPartner([FromServices] IHandler<AddContactToPartnerCommand, ContactDTO> handler,
        [FromBody] AddContactToPartnerCommand command,  Guid partnerId, CancellationToken cancellationToken)
    {
        command.SetPartnerId(partnerId);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }

    [HttpDelete("/api/v1/Clients/{clientId:guid}/Contacts/{contactId:guid}", Name = "RemoveContactFromClient")]
    public async Task<IActionResult> RemoveContactFromClient([FromServices] IHandler<RemoveContactFromClientCommand, ContactDTO> handler,
        Guid clientId, Guid contactId, CancellationToken cancellationToken)
    {
        var command = new RemoveContactFromClientCommand(clientId, contactId);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpDelete("/api/v1/Partners/{partnerId:guid}/Contacts/{contactId:guid}", Name = "RemoveContactFromPartner")]
    public async Task<IActionResult> RemoveContactFromPartner([FromServices] IHandler<RemoveContactFromPartnerCommand, ContactDTO> handler,
        Guid partnerId, Guid contactId, CancellationToken cancellationToken)
    {
        var command = new RemoveContactFromPartnerCommand(partnerId, contactId);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPatch("{id:guid}", Name = "EditContact")]
    public async Task<IActionResult> EditContact([FromServices] IHandler<EditContactCommand, ContactDTO> handler,
        [FromBody] EditContactCommand command, Guid id, CancellationToken cancellationToken)
    {
        command.SetId(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
           ? Ok(response.Success)
           : BadRequest(response.Failure);
    }
}