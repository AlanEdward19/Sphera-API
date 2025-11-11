using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    
    public async Task<Uri> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        fileStream.Position = 0;
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return blobClient.Uri;
    }

    public async Task<Stream?> DownloadAsync(string blobUri, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUri));
        if (!await blobClient.ExistsAsync(cancellationToken)) return null;
        var response = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        return response;
    }

    public async Task DeleteAsync(string blobUri, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUri));
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}