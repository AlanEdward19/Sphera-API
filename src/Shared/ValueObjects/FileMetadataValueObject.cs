namespace Sphera.API.Shared.ValueObjects;

public sealed record FileMetadataValueObject
{
    public string FileName { get; init; }
    public long Size { get; init; }
    public string ContentType { get; init; }
    public Uri? BlobUri { get; init; }
    public string Path => BlobUri?.ToString() ?? string.Empty;

    public FileMetadataValueObject(string fileName, long size, string? contentType, Uri? blobUri = null)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new DomainException("Nome do arquivo obrigatório.");
        FileName = fileName;
        Size = size;
        ContentType = contentType ?? "application/octet-stream";
        BlobUri = blobUri;
    }
}
