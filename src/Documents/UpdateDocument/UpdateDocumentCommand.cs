using System.ComponentModel.DataAnnotations;
using Sphera.API.Documents.Enums;

namespace Sphera.API.Documents.UpdateDocument;

/// <summary>
/// Represents a command to update an existing document with new or modified information.
/// </summary>
/// <remarks>This class is typically used as a data transfer object in operations that update document records.
/// All required properties must be set before submitting the command. The class does not perform validation beyond data
/// annotations; callers should ensure that all required fields are populated and valid.</remarks>
public class UpdateDocumentCommand
{
    

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    private Guid _id;

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
    
    [Required]
    public EDocumentProgressStatus ProgressStatus { get; set; }

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => _id = id;
    
    public Guid GetId() => _id;
}
