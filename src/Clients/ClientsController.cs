using Microsoft.AspNetCore.Mvc;

namespace Sphera.API.Clients;

[ApiController]
[Route("api/v1/[controller]")]
public class ClientsController : ControllerBase
{
    [HttpGet(Name = "GetClients")]
    public async Task<IActionResult> GetClients()
    {
        return Ok();
    }

    [HttpGet("{id:guid}", Name = "GetClientById")]
    public async Task<IActionResult> GetClientById(Guid id)
    {
        return Ok();
    }

    [HttpPost(Name = "CreateClient")]
    public async Task<IActionResult> CreateClient()
    {
        return Created();
    }
    
    [HttpPut("{id:guid}", Name = "UpdateClient")]
    public async Task<IActionResult> UpdateClient(Guid id)
    {
        return Ok();
    }

    [HttpPatch("{id:guid}/activate", Name = "ActivateClient")]
    public async Task<IActionResult> ActivateClient(Guid id)
    {
        return Ok();
    }
    
    [HttpPatch("{id:guid}/deactivate", Name = "DeactivateClient")]
    public async Task<IActionResult> DeactivateClient(Guid id)
    {
        return Ok();
    }
    
    [HttpDelete("{id:guid}", Name = "DeleteClient")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        return NoContent();
    }
}