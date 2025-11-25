using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sphera.API.Auditory;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuditoryController : ControllerBase
{
    [HttpGet(Name = "GetAuditories")]
    public async Task<IActionResult> GetAuditories()
    {
        return Ok();
    }
}