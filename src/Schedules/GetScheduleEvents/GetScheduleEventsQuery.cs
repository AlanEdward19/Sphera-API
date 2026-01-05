using Sphera.API.Schedules.Enums;

namespace Sphera.API.Schedules.GetScheduleEvents;

public class GetScheduleEventsQuery
{
    /// <summary>
    /// Gets or sets the scheduled start date and time for the operation.
    /// </summary>
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event or period ends.
    /// </summary>
    public DateTime? EndAt { get; set; }
    
    public EScheduleEventType? EventType { get; set; }
    
    public Guid? CreatedBy { get; set; }
}