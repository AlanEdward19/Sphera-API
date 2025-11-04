using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Services.UpdateService;

/// <summary>
/// Represents a command to update the properties of a service entity, including its unique identifier, name, and
/// default due period.
/// </summary>
public class UpdateServiceCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(120)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the default number of days until an item is due.
    /// </summary>
    [Range(1, 31)]
    public short DefaultDueInDays { get; set; }

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => Id = id;
}
