using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.External.Storage;

public class SpheraStorage : IStorage
{
    private readonly BlobContainerClient _containerClient;
    
    public SpheraStorage(string connectionString, string containerName)
    {
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<(BlobClient blobClient, Uri sasUri)?> GetBlobClientWithSasAsync(string fileName, TimeSpan? validity = null,
        CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync(cancellationToken)) return null;

        var expiresAt = DateTimeOffset.UtcNow.Add(validity ?? TimeSpan.FromMinutes(10));
        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiresAt);
        return (blobClient, sasUri);
    }

    public async Task<Uri> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        fileStream.Position = 0;
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        return sasUri;
    }

    public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}