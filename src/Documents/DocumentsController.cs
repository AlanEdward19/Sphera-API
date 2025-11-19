using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Documents.CreateDocument;
using Sphera.API.Documents.DTOs;
using Sphera.API.Documents.GetDocuments;
using Sphera.API.Documents.UploadDocument;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Documents;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    [HttpGet(Name = "GetDocuments")]
    public async Task<IActionResult> GetDocuments(
        [FromServices] IHandler<GetDocumentsQuery, IEnumerable<DocumentWithMetadataDTO>> handler,
        [FromQuery] GetDocumentsQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpGet("{id:guid}", Name = "GetDocumentById")]
    public async Task<IActionResult> GetDocumentById(Guid id)
    {
        return Ok();
    }

    [HttpPost(Name = "CreateDocument")]
    public async Task<IActionResult> CreateDocument([FromServices] IHandler<CreateDocumentCommand, DocumentDTO> handler,
        [FromBody] CreateDocumentCommand command, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPut("{id:guid}", Name = "UpdateDocument")]
    public async Task<IActionResult> UpdateDocument(Guid id)
    {
        return Ok();
    }

    [HttpDelete(Name = "DeleteDocument")]
    public async Task<IActionResult> DeleteDocument()
    {
        return NoContent();
    }

    [HttpPost("{id:guid}/upload", Name = "UploadDocument")]
    public async Task<IActionResult> UploadDocument([FromServices] IHandler<UploadDocumentCommand, bool> handler,
        Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        var fileMemoryStream = new MemoryStream();
        await file.CopyToAsync(fileMemoryStream, cancellationToken);

        UploadDocumentCommand command = new();

        command.SetId(id);
        command.SetData(fileMemoryStream);

        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }


    [HttpGet("statuses", Name = "GetDocumentStatuses")]
    public async Task<IActionResult> GetDocumentStatuses()
    {
        return Ok();
    }
}