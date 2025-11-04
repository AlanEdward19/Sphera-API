using Microsoft.AspNetCore.Mvc;

namespace Sphera.API.Services;

[ApiController]
[Route("api/v1/[controller]")]
public class ServicesController : ControllerBase
{
    [HttpGet(Name = "GetServices")]
    public async Task<IActionResult> GetServices()
    {
        return Ok();
    }

    [HttpGet("{id:guid}", Name = "GetServiceById")]
    public async Task<IActionResult> GetServiceById(Guid id)
    {
        return Ok();
    }

    [HttpPost(Name = "CreateService")]
    public async Task<IActionResult> CreateService()
    {
        return Created();
    }

    [HttpPut("{id:guid}", Name = "UpdateService")]
    public async Task<IActionResult> UpdateService(Guid id)
    {
        return Ok();
    }

    [HttpPatch("{id:guid}/activate", Name = "ActivateService")]
    public async Task<IActionResult> ActivateService(Guid id)
    {
        return Ok();
    }
    
    [HttpPatch("{id:guid}/deactivate", Name = "DeactivateService")]
    public async Task<IActionResult> DeactivateService(Guid id)
    {
        return Ok();
    }
    
    [HttpDelete("{id:guid}", Name = "DeleteService")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        return NoContent();
    }
}