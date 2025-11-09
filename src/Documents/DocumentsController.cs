using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sphera.API.Documents;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    [HttpGet(Name = "GetDocuments")]
    public async Task<IActionResult> GetDocuments()
    {
        return Ok();
    }
    
    [HttpGet("{id:guid}", Name = "GetDocumentById")]
    public async Task<IActionResult> GetDocumentById(Guid id)
    {
        return Ok();
    }
    
    [HttpPost(Name = "CreateDocument")]
    public async Task<IActionResult> CreateDocument()
    {
        return Ok();
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
    public async Task<IActionResult> UploadDocument(Guid id)
    {
        return Created();
    }
    
    [HttpGet("statuses", Name = "GetDocumentStatuses")]
    public async Task<IActionResult> GetDocumentStatuses()
    {
        return Ok();
    }
}