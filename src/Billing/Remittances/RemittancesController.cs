using Microsoft.AspNetCore.Mvc;
using Sphera.API.Billing.Remittances.CreateRemittance;
using Sphera.API.Billing.Remittances.DeleteRemittance;
using Sphera.API.Billing.Remittances.DTOs;
using Sphera.API.Billing.Remittances.GetRemittanceById;
using Sphera.API.Billing.Remittances.ListRemittances;
using Sphera.API.Billing.Remittances.SubmitRemittance;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Remittances;

[ApiController]
[Route("api/v1/billing/[controller]")]
public class RemittancesController : ControllerBase
{
    [HttpGet(Name = "ListRemittances")]
    public async Task<IActionResult> List(
        [FromServices] IHandler<ListRemittancesQuery, IReadOnlyCollection<RemittanceDTO>> handler,
        [FromQuery] ListRemittancesQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetRemittanceById")]
    public async Task<IActionResult> GetById(
        [FromServices] IHandler<GetRemittanceByIdQuery, RemittanceDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetRemittanceByIdQuery { Id = id }, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpPost(Name = "CreateRemittance")]
    public async Task<IActionResult> Create(
        [FromServices] IHandler<CreateRemittanceCommand, RemittanceDTO> handler,
        [FromBody] CreateRemittanceCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created(HttpContext.Request.Path, response.Success)
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpPost("{id:guid}/submit", Name = "SubmitRemittance")]
    public async Task<IActionResult> Submit(
        [FromServices] IHandler<SubmitRemittanceCommand, RemittanceDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new SubmitRemittanceCommand { Id = id }, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpDelete("{id:guid}", Name = "DeleteRemittance")]
    public async Task<IActionResult> Delete(
        [FromServices] IHandler<DeleteRemittanceCommand, bool> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new DeleteRemittanceCommand { Id = id }, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : StatusCode(response.Failure!.Code, response.Failure);
    }
}
