namespace Sphera.API.Schedules.GetUserScheduleEvents;

/// <summary>
/// Represents a query for retrieving scheduled events associated with a specific user within an optional date and time
/// range.
/// </summary>
public class GetUserScheduleEventsQuery
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the scheduled start date and time for the operation.
    /// </summary>
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event or period ends.
    /// </summary>
    public DateTime? EndAt { get; set; }
}