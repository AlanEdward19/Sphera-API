namespace Sphera.API.Schedules;

public class ScheduleEventDTO
{
    /// <summary>
    /// Gets the unique identifier of the schedule event.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the client.
    /// </summary>
    public Guid ClientId { get; private set; }

    /// <summary>
    /// Gets any additional notes or comments related to the event.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the date and time when the event was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who created the event.
    /// </summary>
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
    /// Initializes a new instance of the ScheduleEventDTO class with the specified event and audit information.
    /// </summary>
    /// <param name="id">The unique identifier for the schedule event.</param>
    /// <param name="occurredAt">The date and time when the event occurred.</param>
    /// <param name="userId">The unique identifier of the user associated with the event.</param>
    /// <param name="clientId">The unique identifier of the client associated with the event.</param>
    /// <param name="notes">Optional notes or comments related to the event. Can be null.</param>
    /// <param name="createdAt">The date and time when the event record was created.</param>
    /// <param name="createdBy">The unique identifier of the user who created the event record.</param>
    /// <param name="updatedAt">The date and time when the event record was last updated, or null if it has not been updated.</param>
    /// <param name="updatedBy">The unique identifier of the user who last updated the event record, or null if it has not been updated.</param>
    public ScheduleEventDTO(Guid id, DateTime occurredAt, Guid userId, Guid clientId, string? notes, DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy)
    {
        Id = id;
        OccurredAt = occurredAt;
        UserId = userId;
        ClientId = clientId;
        Notes = notes;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }
}