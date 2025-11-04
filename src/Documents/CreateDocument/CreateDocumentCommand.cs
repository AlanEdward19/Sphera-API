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
    public Guid ClientId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the partner associated with this entity.
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the service.
    /// </summary>
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the responsible party.
    /// </summary>
    public Guid ResponsibleId { get; set; }

    /// <summary>
    /// Gets or sets the date when the item was issued.
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time by which the item is due.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes or comments associated with the object.
    /// </summary>
    public string? Notes { get; set; }
}
