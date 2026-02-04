using Microsoft.AspNetCore.Mvc;
using Sphera.API.Billing.BilletConfigurations.CreateBilletConfiguration;
using Sphera.API.Billing.BilletConfigurations.DeleteBilletConfiguration;
using Sphera.API.Billing.BilletConfigurations.DTOs;
using Sphera.API.Billing.BilletConfigurations.GetBilletConfigurationById;
using Sphera.API.Billing.BilletConfigurations.ListBilletConfigurations;
using Sphera.API.Billing.BilletConfigurations.UpdateBilletConfiguration;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.BilletConfigurations;

[ApiController]
[Route("api/v1/billing/[controller]")]
public class BilletConfigurationsController : ControllerBase
{
    [HttpGet(Name = "ListBilletConfigurations")]
    public async Task<IActionResult> List(
        [FromServices] IHandler<ListBilletConfigurationsQuery, IReadOnlyCollection<BilletConfigurationDTO>> handler,
        [FromQuery] ListBilletConfigurationsQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetBilletConfigurationById")]
    public async Task<IActionResult> GetById(
        [FromServices] IHandler<GetBilletConfigurationByIdQuery, BilletConfigurationDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetBilletConfigurationByIdQuery { Id = id }, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    [HttpPost(Name = "CreateBilletConfiguration")]
    public async Task<IActionResult> Create(
        [FromServices] IHandler<CreateBilletConfigurationCommand, BilletConfigurationDTO> handler,
        [FromBody] CreateBilletConfigurationCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Created(HttpContext.Request.Path, response.Success)
            : StatusCode(response.Failure.Code, response.Failure);
    }

    [HttpPut("{id:guid}", Name = "UpdateBilletConfiguration")]
    public async Task<IActionResult> Update(
        [FromServices] IHandler<UpdateBilletConfigurationCommand, BilletConfigurationDTO> handler,
        Guid id,
        [FromBody] UpdateBilletConfigurationCommand command,
        CancellationToken cancellationToken)
    {
        command.SetId(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    [HttpDelete("{id:guid}", Name = "DeleteBilletConfiguration")]
    public async Task<IActionResult> Delete(
        [FromServices] IHandler<DeleteBilletConfigurationCommand, bool> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new DeleteBilletConfigurationCommand { Id = id }, HttpContext, cancellationToken);
        return response.IsSuccess ? NoContent() : StatusCode(response.Failure.Code, response.Failure);
    }
}

