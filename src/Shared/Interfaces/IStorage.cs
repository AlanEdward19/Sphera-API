namespace Sphera.API.Shared.Interfaces;

public interface IStorage
{
    Task<Uri> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string blobUri, CancellationToken cancellationToken = default);
    Task DeleteAsync(string blobUri, CancellationToken cancellationToken = default);
}