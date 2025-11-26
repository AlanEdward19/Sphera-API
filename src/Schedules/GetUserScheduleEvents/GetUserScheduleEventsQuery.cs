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
    private Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the scheduled start date and time for the operation.
    /// </summary>
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event or period ends.
    /// </summary>
    public DateTime? EndAt { get; set; }

    /// <summary>
    /// Sets the unique identifier for the user associated with this instance.
    /// </summary>
    /// <param name="userId">The unique identifier to assign to the user. Must not be <see cref="Guid.Empty"/>.</param>
    public void SetUserId(Guid userId) => UserId = userId;

    /// <summary>
    /// Gets the unique identifier for the current user.
    /// </summary>
    /// <returns>A <see cref="System.Guid"/> representing the unique identifier of the user.</returns>
    public Guid GetUserId() => UserId;
}