using System.ComponentModel.DataAnnotations;
using Sphera.API.Schedules.Enums;

namespace Sphera.API.Schedules.CreateScheduleEvent;

/// <summary>
/// Command to create a new schedule event.
/// </summary>
public class CreateScheduleEventCommand
{
    /// <summary>
    /// Gets or sets the date and time when the event occurred.
    /// </summary>
    [Required]
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the type of the event (Individual or Global).
    /// </summary>
    [Required]
    public EScheduleEventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user (optional).
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the client (optional).
    /// </summary>
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Gets or sets any additional notes or comments related to the event.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}