using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Clients;
using Sphera.API.Schedules.Enums;
using Sphera.API.Shared;
using Sphera.API.Users;

namespace Sphera.API.Schedules;

public class ScheduleEvent
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    [Required]
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Gets the type of the event (Individual or Global).
    /// </summary>
    [Required]
    public EScheduleEventType EventType { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the user. Optional.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the client. Optional.
    /// </summary>
    public Guid? ClientId { get; private set; }

    /// <summary>
    /// Gets the optional notes or comments associated with this instance.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who created the entity.
    /// </summary>
    [Required]
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; private set; }

    /// <summary>
    /// Gets the version of the row used for concurrency control.
    /// </summary>
    /// <remarks>This property is typically used to detect and prevent conflicting updates in optimistic
    /// concurrency scenarios. The value is updated automatically by the data store when the row is modified.</remarks>
    public byte[] RowVersion { get; private set; }

    /// <summary>
    /// Gets the user associated with this entity.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; private set; }

    /// <summary>
    /// Gets the client associated with this entity.
    /// </summary>
    [ForeignKey(nameof(ClientId))]
    public virtual Client? Client { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ScheduleEvent class.
    /// </summary>
    /// <remarks>This constructor is private and is intended for internal use only. Instances of ScheduleEvent
    /// cannot be created directly from outside the class.</remarks>
    private ScheduleEvent()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ScheduleEvent class with the specified occurrence time, user, client, creator, and
    /// optional notes.
    /// </summary>
    /// <param name="occurredAt">The date and time when the event occurred.</param>
    /// <param name="eventType">The type of the event (Individual or Global).</param>
    /// <param name="userId">The unique identifier of the user associated with the event. Cannot be null or empty.</param>
    /// <param name="clientId">The unique identifier of the client associated with the event. Cannot be null or empty.</param>
    /// <param name="createdBy">The unique identifier of the user who created the event.</param>
    /// <param name="notes">Optional notes or comments related to the event. If null or whitespace, the value is ignored.</param>
    /// <remarks>Both <paramref name="userId"/> and <paramref name="clientId"/> are optional.</remarks>
    public ScheduleEvent(DateTime occurredAt, EScheduleEventType eventType, Guid? userId, Guid? clientId, Guid createdBy, string? notes = null)
    {
        Id = Guid.NewGuid();
        OccurredAt = occurredAt;
        EventType = eventType;
        UserId = userId is null || userId == Guid.Empty ? null : userId;
        ClientId = clientId is null || clientId == Guid.Empty ? null : clientId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Updates the event details with the specified occurrence time, user, client, notes, and actor information.
    /// </summary>
    /// <param name="occurredAt">The date and time when the event occurred.</param>
    /// <param name="eventType">The type of the event (Individual or Global).</param>
    /// <param name="userId">The unique identifier of the user associated with the event. Cannot be null or empty.</param>
    /// <param name="clientId">The unique identifier of the client associated with the event. Cannot be null or empty.</param>
    /// <param name="notes">Optional notes or comments related to the event. If null or whitespace, notes will be cleared.</param>
    /// <param name="actor">The unique identifier of the user performing the update.</param>
    public void Update(DateTime occurredAt, EScheduleEventType eventType, Guid? userId, Guid? clientId, string? notes, Guid actor)
    {
        OccurredAt = occurredAt;
        EventType = eventType;
        UserId = userId is null || userId == Guid.Empty ? null : userId;
        ClientId = clientId is null || clientId == Guid.Empty ? null : clientId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public ScheduleEventDTO ToDTO()
    {
        return new ScheduleEventDTO(
            Id,
            OccurredAt,
            EventType,
            UserId,
            ClientId,
            Notes,
            CreatedAt,
            CreatedBy,
            UpdatedAt,
            UpdatedBy
        );
    }
}