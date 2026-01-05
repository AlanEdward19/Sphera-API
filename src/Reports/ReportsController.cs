using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Reports.GenerateClientsReport;
using Sphera.API.Reports.GenerateFilesReport;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Reports;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    [HttpGet("Clients", Name = "GenerateClientsReport")]
    public async Task<IActionResult> GenerateClientsReport(
        [FromServices] IHandler<GenerateClientsReportQuery, ClientsReportDTO[]> handler,
        [FromQuery] GenerateClientsReportQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure!.Code, response.Failure);
    }
    
    [HttpGet("Files", Name = "GenerateFilesReport")]
    public async Task<IActionResult> GenerateFilesReport(
        [FromServices] IHandler<GenerateFilesReportQuery, FilesReportDTO[]> handler,
        [FromQuery] GenerateFilesReportQuery query, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure!.Code, response.Failure);
    }
}