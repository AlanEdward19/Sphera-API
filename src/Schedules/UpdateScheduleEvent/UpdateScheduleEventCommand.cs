using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Schedules.UpdateScheduleEvent;

/// <summary>
/// Command to update an existing schedule event.
/// </summary>
public class UpdateScheduleEventCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the object.
    /// </summary>
    private Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event occurred.
    /// </summary>
    [Required]
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user (optional).
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the client (optional).
    /// </summary>
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Gets or sets optional notes or comments associated with the entity.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    /// <returns>A <see cref="System.Guid"/> that uniquely identifies this instance.</returns>
    public Guid GetId() => Id;

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => Id = id;
}