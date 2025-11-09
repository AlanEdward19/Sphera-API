namespace Sphera.API.Services;

/// <summary>
/// Represents a data transfer object that encapsulates information about a service, including identification, status,
/// and audit metadata.
/// </summary>
/// <remarks>This class is typically used to transfer service-related data between application layers or across
/// service boundaries. All properties are read-only and set through the constructor, ensuring immutability after
/// creation.</remarks>
public class ServiceDTO
{
    /// <summary>
    /// Gets the unique identifier for this instance.
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Gets the name associated with the current instance.
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// Gets the code associated with this instance.
    /// </summary>
    public string Code { get; private set; }
    
    /// <summary>
    /// Gets the date and time by which the item is due, if specified.
    /// </summary>
    public DateTime? DueDate { get; private set; }
    
    /// <summary>
    /// Gets the number of days remaining until the expiration date, if available.
    /// </summary>
    public int? RemainingDays { get; private set; }
    
    /// <summary>
    /// Gets a value indicating whether the object is currently active.
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the object was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Gets the unique identifier of the user who created the entity.
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
    /// Initializes a new instance of the ServiceDTO class with the specified service details.
    /// </summary>
    /// <param name="id">The unique identifier for the service.</param>
    /// <param name="name">The name of the service.</param>
    /// <param name="code">The code that uniquely identifies the service within the system.</param>
    /// <param name="dueDate">The due date for the service, or null if not applicable.</param>
    /// <param name="remainingDays">The number of days remaining until the due date, or null if not applicable.</param>
    /// <param name="isActive">A value indicating whether the service is currently active.</param>
    /// <param name="createdAt">The date and time when the service was created.</param>
    /// <param name="createdBy">The unique identifier of the user who created the service.</param>
    /// <param name="updatedAt">The date and time when the service was last updated, or null if it has not been updated.</param>
    /// <param name="updatedBy">The unique identifier of the user who last updated the service, or null if it has not been updated.</param>
    public ServiceDTO(Guid id, string name, string code, DateTime? dueDate, int? remainingDays, bool isActive, DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy)
    {
        Id = id;
        Name = name;
        Code = code;
        DueDate = dueDate;
        RemainingDays = remainingDays;
        IsActive = isActive;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }
}