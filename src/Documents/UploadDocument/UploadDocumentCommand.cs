namespace Sphera.API.Documents.UploadDocument;

public class UploadDocumentCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    private Guid _id;

    private long _size;
    private string _contentType;
    private MemoryStream _data;

    public Guid GetId() => _id;
    public void SetId(Guid id) => _id = id;
    
    public long GetSize() => _size;
    public void SetSize(long size) => _size = size;
    
    public string GetContentType() => _contentType;
    public void SetContentType(string contentType) => _contentType = contentType;
    
    public MemoryStream GetData() => _data;
    public void SetData(MemoryStream data) => _data = data;
}