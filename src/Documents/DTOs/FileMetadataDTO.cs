namespace Sphera.API.Documents.DTOs;

public record FileMetadataDTO(string FileName, long Size, string ContentType, string BlobUri);