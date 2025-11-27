namespace Sphera.API.Schedules.DeleteScheduleEvent;

/// <summary>
/// Represents a command to request the deletion of a scheduled event by its unique identifier.
/// </summary>
public class DeleteScheduleEventCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    private Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the DeleteScheduleEventCommand class with the specified event identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the schedule event to delete.</param>
    public DeleteScheduleEventCommand(Guid id) => Id = id;

    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    /// <returns>A <see cref="Guid"/> value that uniquely identifies this instance.</returns>
    public Guid GetId() => Id;
}