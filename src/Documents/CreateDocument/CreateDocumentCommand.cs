using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Documents.CreateDocument;

/// <summary>
/// Represents a command to create a new document with associated client, partner, service, and responsible party
/// information.
/// </summary>
public class CreateDocumentCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the client.
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the service.
    /// </summary>
    [Required]
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the responsible party.
    /// </summary>
    [Required]
    public Guid ResponsibleId { get; set; }

    /// <summary>
    /// Gets or sets the date when the item was issued.
    /// </summary>
    [Required]
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time by which the item is due.
    /// </summary>
    [Required]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes or comments associated with the object.
    /// </summary>
    [MaxLength(255)]
    public string? Notes { get; set; }
}
