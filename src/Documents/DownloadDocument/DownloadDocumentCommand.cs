namespace Sphera.API.Documents.DownloadDocument;

public class DownloadDocumentCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    private Guid _id;
    
    public Guid GetId() => _id;
    public void SetId(Guid id) => _id = id;
}