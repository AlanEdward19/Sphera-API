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
    private Guid Id { get; set; }

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
    public short? DefaultDueInDays { get; set; }

    /// <summary>
    /// Gets or sets the optional notes or comments associated with this instance.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets the unique identifier for the current instance.
    /// </summary>
    /// <returns>A <see cref="Guid"/> representing the unique identifier of this instance.</returns>
    public Guid GetId() => Id;

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => Id = id;
}
