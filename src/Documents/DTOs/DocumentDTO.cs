using Sphera.API.Shared.Enums;

namespace Sphera.API.Documents.DTOs;

public class DocumentDTO
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Guid ResponsibleId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public EExpirationStatus Status { get; private set; }
    
    public DocumentDTO(Guid id, string fileName, Guid clientId, Guid serviceId, Guid responsibleId, DateTime issueDate, DateTime dueDate, string? notes, DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy, EExpirationStatus status)
    {
        Id = id;
        FileName = fileName;
        ClientId = clientId;
        ServiceId = serviceId;
        ResponsibleId = responsibleId;
        IssueDate = issueDate;
        DueDate = dueDate;
        Notes = notes;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
        Status = status;
    }
}