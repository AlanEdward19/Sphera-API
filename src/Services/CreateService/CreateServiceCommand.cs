using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Services.CreateService;

/// <summary>
/// Represents a command to create a new service with specified properties.
/// </summary>
public class CreateServiceCommand
{
    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(120)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the code associated with this instance.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(40)]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the default number of days until an item is due.
    /// </summary>
    public short? DefaultDueInDays { get; set; }
}
