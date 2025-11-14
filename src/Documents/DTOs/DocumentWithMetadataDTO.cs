using Sphera.API.Documents.Common.Enums;

namespace Sphera.API.Documents.DTOs;

public class DocumentWithMetadataDTO(
    Guid id,
    string fileName,
    Guid clientId,
    Guid serviceId,
    Guid responsibleId,
    DateTime issueDate,
    DateTime dueDate,
    string? notes,
    DateTime createdAt,
    Guid createdBy,
    DateTime? updatedAt,
    Guid? updatedBy,
    EDocumentStatus status,
    string partnerName,
    string clientName,
    string serviceName,
    string responsibleName,
    FileMetadataDTO? fileMetadata)
    : DocumentDTO(id, fileName, clientId, serviceId, responsibleId, issueDate, dueDate, notes, createdAt, createdBy, updatedAt,
        updatedBy, status)
{
    public string PartnerName { get; } = partnerName;
    public string ClientName { get; } = clientName;
    public string ServiceName { get; } = serviceName;
    public string ResponsibleName { get;  } = responsibleName;
    public FileMetadataDTO? FileMetadata { get; } = fileMetadata;
}