using Microsoft.AspNetCore.Mvc;

namespace Sphera.API.Partners;

[ApiController]
[Route("api/v1/[controller]")]
public class PartnersController : ControllerBase
{
    [HttpGet(Name = "GetPartners")]
    public async Task<IActionResult> GetPartners()
    {
        return Ok();
    }

    [HttpGet("{id:guid}", Name = "GetPartnerById")]
    public async Task<IActionResult> GetPartnerById(Guid id)
    {
        return Ok();
    }
    
    [HttpPost(Name = "CreatePartner")]
    public async Task<IActionResult> CreatePartner()
    {
        return Created();
    }

    [HttpPut("{id:guid}", Name = "UpdatePartner")]
    public async Task<IActionResult> UpdatePartner(Guid id)
    {
        return Ok();
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
    public async Task<IActionResult> DeletePartner(Guid id)
    {
        return NoContent();
    }
}