using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Auditory.GetAuditories;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Auditory;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuditoryController : ControllerBase
{
    [HttpGet(Name = "GetAuditories")]
    public async Task<IActionResult> GetAuditories(
        [FromServices] IHandler<GetAuditoriesQuery, IEnumerable<AuditoryDTO>> handler,
        [FromQuery] GetAuditoriesQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }
}