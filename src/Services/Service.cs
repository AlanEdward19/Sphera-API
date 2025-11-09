using System.ComponentModel.DataAnnotations;
using Sphera.API.Shared;

namespace Sphera.API.Services;

/// <summary>
/// Represents a business service with identifying information, status, and audit metadata.
/// </summary>
/// <remarks>The Service class encapsulates the core properties and state transitions for a business service
/// entity, including creation, updates, activation, and deactivation. It maintains audit information such as creation
/// and update timestamps and user identifiers. The class enforces validation on required fields and ensures that the
/// service code is unique within the system. Instances of this class are typically created and managed through
/// application logic that enforces business rules.</remarks>
public class Service
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the name associated with this instance.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(120)]
    public string Name { get; private set; }

    /// <summary>
    /// Gets the code associated with this instance.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(40)]
    public string Code { get; private set; }

    /// <summary>
    /// Gets the due date for the item.
    /// </summary>
    public DateTime? DueDate { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the entity is active.
    /// </summary>
    [Required]
    public bool IsActive { get; private set; }

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
    /// <remarks>The row version is typically used to detect conflicting updates in optimistic concurrency
    /// scenarios. The value is updated automatically by the data store each time the row is modified.</remarks>
    public byte[] RowVersion { get; private set; }

    /// <summary>
    /// EF Core parameterless constructor.
    /// </summary>
    private Service() { }

    /// <summary>
    /// Initializes a new instance of the Service class with the specified name, code, due date, and creator identifier.
    /// </summary>
    /// <param name="name">The name of the service. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="code">The unique code identifying the service. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="dueDate">The due date for the service.</param>
    /// <param name="createdBy">The unique identifier of the user who created the service.</param>
    /// <exception cref="DomainException">Thrown if name or code is null, empty, or consists only of white-space characters.</exception>
    public Service(string name, string code, DateTime? dueDate, Guid createdBy)
    {
        Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome do serviço obrigatório.");
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Código do serviço obrigatório.");

        Name = name;
        Code = code;
        DueDate = dueDate;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Updates the name and due date of the item if new values are provided, and records the actor and update
    /// timestamp.
    /// </summary>
    /// <remarks>The method updates the item's last modified timestamp and actor regardless of whether the
    /// name or due date are changed.</remarks>
    /// <param name="name">The new name to assign to the item. If null or whitespace, the name is not changed.</param>
    /// <param name="dueDate">The new due date to assign to the item. If null, the due date is not changed.</param>
    /// <param name="actor">The unique identifier of the actor performing the update. This value is recorded as the updater.</param>
    public void Update(string? name, DateTime? dueDate, Guid actor)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;

        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Activates the entity and records the actor responsible for the activation.
    /// </summary>
    /// <param name="actor">The unique identifier of the actor performing the activation. This value is used to update the audit information
    /// for the entity.</param>
    public void Activate(Guid actor) { IsActive = true; UpdatedAt = DateTime.UtcNow; UpdatedBy = actor; }

    /// <summary>
    /// Deactivates the current entity and records the actor responsible for the change.
    /// </summary>
    /// <param name="actor">The unique identifier of the actor performing the deactivation. This value is used to update the audit
    /// information.</param>
    public void Deactivate(Guid actor) { IsActive = false; UpdatedAt = DateTime.UtcNow; UpdatedBy = actor; }
    
    /// <summary>
    /// Converts the current service entity to a corresponding data transfer object (DTO).
    /// </summary>
    /// <returns>A <see cref="ServiceDTO"/> instance containing the data from the current service entity.</returns>
    public ServiceDTO ToDTO()
    {
        return new ServiceDTO(Id, Name, Code, DueDate, DueDate.HasValue ? (DueDate.Value - DateTime.Today).Days : null,
            IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);
    }
}
