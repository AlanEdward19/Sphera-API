using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sphera.API.Schedules.CreateScheduleEvent;
using Sphera.API.Schedules.DeleteScheduleEvent;
using Sphera.API.Schedules.GetUserScheduleEvents;
using Sphera.API.Schedules.UpdateScheduleEvent;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Schedules;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    [HttpPost(Name = "CreateScheduleEvent")]
    public async Task<IActionResult> Create([FromServices] IHandler<CreateScheduleEventCommand, ScheduleEventDTO> handler,
        [FromBody] CreateScheduleEventCommand command, CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created(HttpContext.Request.GetDisplayUrl(), response.Success)
            : BadRequest(response.Failure);
    }

    [HttpGet("user/{userId:guid}", Name = "GetUserScheduleEvents")]
    public async Task<IActionResult> GetUserEvents([FromServices] IHandler<GetUserScheduleEventsQuery, IEnumerable<ScheduleEventDTO>> handler,
        Guid userId, [FromQuery] GetUserScheduleEventsQuery query, CancellationToken cancellationToken)
    {
        query.SetUserId(userId);
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpPut("{id:guid}", Name = "UpdateScheduleEvent")]
    public async Task<IActionResult> Update([FromServices] IHandler<UpdateScheduleEventCommand, ScheduleEventDTO> handler,
        [FromBody] UpdateScheduleEventCommand command, Guid id, CancellationToken cancellationToken)
    {
        command.SetId(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : BadRequest(response.Failure);
    }

    [HttpDelete("{id:guid}", Name = "DeleteScheduleEvent")]
    public async Task<IActionResult> Delete([FromServices] IHandler<DeleteScheduleEventCommand, bool> handler, Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteScheduleEventCommand(id);
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : BadRequest(response.Failure);
    }
}
