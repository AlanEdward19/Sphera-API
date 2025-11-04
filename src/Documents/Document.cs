using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Documents.Common.Enums;
using Sphera.API.Services;
using Sphera.API.Shared;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Documents;

public class Document
{
    [Key]
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Guid ResponsibleId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public FileMetadataValueObject File { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public byte[] RowVersion { get; private set; }
    public EDocumentStatus Status => ComputeStatus();

    [ForeignKey(nameof(ServiceId))] public virtual Service Service { get; private set; }

    private Document() { }

    public Document(Guid id, Guid clientId, Guid serviceId, Guid responsibleId, DateTime issueDate, DateTime dueDate,
        FileMetadataValueObject file, Guid createdBy, string? notes = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        if (clientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");
        if (serviceId == Guid.Empty) throw new DomainException("ServiceId obrigatório.");

        if (string.IsNullOrWhiteSpace(responsibleId.ToString()) || responsibleId.Equals(Guid.Empty))
            throw new DomainException("Responsável obrigatório.");

        if (issueDate > dueDate) throw new DomainException("Data de emissão não pode ser posterior ao vencimento.");

        ClientId = clientId;
        ServiceId = serviceId;
        ResponsibleId = responsibleId;
        IssueDate = issueDate;
        DueDate = dueDate;
        File = file ?? throw new DomainException("Arquivo obrigatório para documento.");
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    private EDocumentStatus ComputeStatus()
    {
        var now = DateTime.UtcNow.Date;
        if (DueDate.Date < now) return EDocumentStatus.Expired;

        var daysLeft = (DueDate.Date - now).TotalDays;

        return daysLeft <= Service.DefaultDueInDays ? EDocumentStatus.AboutToExpire : EDocumentStatus.WithinDeadline;
    }

    public void UpdateFile(FileMetadataValueObject? newFile, Guid actor)
    {
        File = newFile ?? throw new DomainException("Arquivo inválido.");
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void UpdateDates(DateTime issueDate, DateTime dueDate, Guid actor)
    {
        if (issueDate > dueDate) throw new DomainException("Data de emissão não pode ser posterior ao vencimento.");
        IssueDate = issueDate;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void ChangeResponsible(Guid newResponsableId, Guid actor)
    {
        if (string.IsNullOrWhiteSpace(newResponsableId.ToString()) || newResponsableId.Equals(Guid.Empty)) throw new DomainException("Responsável inválido.");
        ResponsibleId = newResponsableId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void AddNotes(string note, Guid actor)
    {
        Notes = string.IsNullOrWhiteSpace(Notes) ? note : $"{Notes}\n{note}";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void ExtendDueDate(DateTime newDueDate, Guid actor)
    {
        if (newDueDate <= DueDate) throw new DomainException("Nova data de vencimento deve ser posterior à atual.");
        DueDate = newDueDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }
}