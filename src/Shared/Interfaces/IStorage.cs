using Azure.Storage.Blobs;

namespace Sphera.API.Shared.Interfaces;

public interface IStorage
{
    Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default);
    Task<(BlobClient blobClient, Uri sasUri)?> GetBlobClientWithSasAsync(string fileName, TimeSpan? validity = null,
        CancellationToken cancellationToken = default);
    Task<Uri> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
}