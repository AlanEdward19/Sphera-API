using Microsoft.AspNetCore.Mvc;
using Sphera.API.Billing.Billets.CreateBillet;
using Sphera.API.Billing.Billets.DeleteBillet;
using Sphera.API.Billing.Billets.DTOs;
using Sphera.API.Billing.Billets.GetBilletById;
using Sphera.API.Billing.Billets.ListBillets;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Billing.Billets.GenerateBilletFile;
using Sphera.API.Billing.Billets.DownloadBilletFile;

namespace Sphera.API.Billing.Billets;

[ApiController]
[Route("api/v1/billing/[controller]")]
public class BilletsController : ControllerBase
{
    [HttpGet(Name = "ListBillets")]
    public async Task<IActionResult> List(
        [FromServices] IHandler<ListBilletsQuery, IReadOnlyCollection<BilletDTO>> handler,
        [FromQuery] ListBilletsQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Ok(response.Success)
            : StatusCode(response.Failure.Code, response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetBilletById")]
    public async Task<IActionResult> GetById(
        [FromServices] IHandler<GetBilletByIdQuery, BilletDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetBilletByIdQuery { Id = id }, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpPost(Name = "CreateBillet")]
    public async Task<IActionResult> Create(
        [FromServices] IHandler<CreateBilletCommand, BilletDTO> handler,
        [FromBody] CreateBilletCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created(HttpContext.Request.Path, response.Success)
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpDelete("{id:guid}", Name = "DeleteBillet")]
    public async Task<IActionResult> Delete(
        [FromServices] IHandler<DeleteBilletCommand, bool> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new DeleteBilletCommand { Id = id }, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpPost("{id:guid}/generate-file", Name = "GenerateBilletFile")]
    public async Task<IActionResult> GenerateFile(
        [FromServices] IHandler<GenerateBilletFileCommand, bool> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GenerateBilletFileCommand { Id = id }, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure!.Code, response.Failure);
    }

    [HttpGet("{id:guid}/download-file", Name = "DownloadBilletFile")]
    public async Task<IActionResult> DownloadFile(
        [FromServices] IHandler<DownloadBilletFileCommand, (Stream, string)> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new DownloadBilletFileCommand { Id = id }, HttpContext, cancellationToken);
        return response.IsSuccess
            ? File(response.Success.Item1, "application/pdf", response.Success.Item2)
            : StatusCode(response.Failure!.Code, response.Failure);
    }
}