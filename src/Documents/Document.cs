using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Clients;
using Sphera.API.Documents.Common.Enums;
using Sphera.API.Documents.CreateDocument;
using Sphera.API.Documents.DTOs;
using Sphera.API.Services;
using Sphera.API.Shared;
using Sphera.API.Shared.ValueObjects;
using Sphera.API.Users;

namespace Sphera.API.Documents;

public class Document
{
    [Key] public Guid Id { get; private set; }

    [Required]
    /// <summary>
    /// Gets the name of the file associated with this entity.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the client.
    /// </summary>
    [Required]
    public Guid ClientId { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    [Required]
    public Guid ServiceId { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the responsible party.
    /// </summary>
    [Required]
    public Guid ResponsibleId { get; private set; }

    /// <summary>
    /// Gets the date and time when the item was issued.
    /// </summary>
    [Required]
    public DateTime IssueDate { get; private set; }

    /// <summary>
    /// Gets the due date for the associated item.
    /// </summary>
    [Required]
    public DateTime DueDate { get; private set; }

    /// <summary>
    /// Gets the optional notes or comments associated with this instance.
    /// </summary>
    [MaxLength(255)]
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who created the entity.
    /// </summary>
    [Required]
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; private set; }

    /// <summary>
    /// Gets the version of the row used for concurrency control.
    /// </summary>
    /// <remarks>The row version is typically used to detect conflicting updates in optimistic concurrency
    /// scenarios. The value is usually set by the data store and should not be modified directly.</remarks>
    public byte[] RowVersion { get; private set; }

    /// <summary>
    /// Gets the current status of the document.
    /// </summary>
    public EDocumentStatus Status => ComputeStatus();

    /// <summary>
    /// Gets the related service entity associated with this instance.
    /// </summary>
    /// <remarks>This property is populated based on the foreign key relationship defined by the ServiceId
    /// property. The value is typically loaded by the Entity Framework when navigating relationships between
    /// entities.</remarks>
    [ForeignKey(nameof(ServiceId))]
    public virtual Service Service { get; private set; }

    /// <summary>
    /// Gets the client associated with this entity.
    /// </summary>
    /// <remarks>This property represents a navigation to the related client in the data model. The value is
    /// populated by the Entity Framework when the entity is loaded from the database.</remarks>
    [ForeignKey(nameof(ClientId))]
    public virtual Client Client { get; private set; }

    [ForeignKey(nameof(ResponsibleId))] 
    public virtual User Responsible { get; private set; }

    /// <summary>
    /// EF Core parameterless constructor.
    /// </summary>
    private Document()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Document class with the specified identifiers, dates, file metadata, creator,
    /// and optional notes.
    /// </summary>
    /// <param name="id">The unique identifier for the document. If Guid.Empty is provided, a new identifier is generated.</param>
    /// <param name="clientId">The unique identifier of the client associated with the document. Cannot be Guid.Empty.</param>
    /// <param name="serviceId">The unique identifier of the service related to the document. Cannot be Guid.Empty.</param>
    /// <param name="responsibleId">The unique identifier of the person responsible for the document. Cannot be Guid.Empty.</param>
    /// <param name="issueDate">The date the document was issued. Must not be later than the due date.</param>
    /// <param name="dueDate">The date by which the document is due. Must not be earlier than the issue date.</param>
    /// <param name="file">The file metadata associated with the document. Cannot be null.</param>
    /// <param name="createdBy">The unique identifier of the user who created the document.</param>
    /// <param name="notes">Optional notes or comments related to the document. Can be null.</param>
    /// <exception cref="DomainException">Thrown if any required parameter is missing or invalid, such as when clientId, serviceId, or responsibleId is
    /// Guid.Empty, file is null, or issueDate is after dueDate.</exception>
    public Document(Guid id, string fileName, Guid clientId, Guid serviceId, Guid responsibleId, DateTime issueDate,
        DateTime dueDate, Guid createdBy, string? notes = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        if (string.IsNullOrWhiteSpace(fileName)) throw new DomainException("Nome do arquivo obrigatório.");
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
        FileName = fileName;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public Document(CreateDocumentCommand command, Guid createdBy)
    {
        Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(command.FileName)) throw new DomainException("Nome do arquivo obrigatório.");
        if (command.ClientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");
        if (command.ServiceId == Guid.Empty) throw new DomainException("ServiceId obrigatório.");

        if (string.IsNullOrWhiteSpace(command.ResponsibleId.ToString()) || command.ResponsibleId.Equals(Guid.Empty))
            throw new DomainException("Responsável obrigatório.");

        if (command.IssueDate > command.DueDate)
            throw new DomainException("Data de emissão não pode ser posterior ao vencimento.");
        FileName = command.FileName;
        ClientId = command.ClientId;
        ServiceId = command.ServiceId;
        ResponsibleId = command.ResponsibleId;
        IssueDate = command.IssueDate;
        DueDate = command.DueDate;
        Notes = command.Notes;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Determines the current status of the document based on its due date and the default due period.
    /// </summary>
    /// <remarks>The status is evaluated using the current UTC date and the document's due date. If the due
    /// date has passed, the status is Expired. If the due date is within the default due period, the status is
    /// AboutToExpire; otherwise, it is WithinDeadline.</remarks>
    /// <returns>An EDocumentStatus value indicating whether the document is expired, about to expire, or within the deadline.</returns>
    private EDocumentStatus ComputeStatus()
    {
        var now = DateTime.UtcNow.Date;
        if (DueDate.Date < now) return EDocumentStatus.Expired;

        var daysLeft = (DueDate.Date - now).TotalDays;

        return daysLeft <= 7 ? EDocumentStatus.AboutToExpire : EDocumentStatus.WithinDeadline;
    }

    /// <summary>
    /// Updates the issue and due dates for the entity and records the actor responsible for the change.
    /// </summary>
    /// <remarks>This method also updates the last modified timestamp and the identifier of the user or
    /// process that performed the update.</remarks>
    /// <param name="issueDate">The new issue date to set. Must not be later than <paramref name="dueDate"/>.</param>
    /// <param name="dueDate">The new due date to set. Must not be earlier than <paramref name="issueDate"/>.</param>
    /// <param name="actor">The unique identifier of the user or process performing the update.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="issueDate"/> is later than <paramref name="dueDate"/>.</exception>
    public void UpdateDates(DateTime issueDate, DateTime dueDate, Guid actor)
    {
        if (issueDate > dueDate) throw new DomainException("Data de emissão não pode ser posterior ao vencimento.");
        IssueDate = issueDate;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Changes the responsible user for the entity to the specified user identifier.
    /// </summary>
    /// <param name="newResponsableId">The unique identifier of the new responsible user. Must not be an empty GUID.</param>
    /// <param name="actor">The unique identifier of the user performing the change.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="newResponsableId"/> is an empty GUID.</exception>
    public void ChangeResponsible(Guid newResponsableId, Guid actor)
    {
        if (string.IsNullOrWhiteSpace(newResponsableId.ToString()) || newResponsableId.Equals(Guid.Empty))
            throw new DomainException("Responsável inválido.");
        ResponsibleId = newResponsableId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }
    
    public void ChangeService(Guid newServiceId, Guid actor)
    {
        if (string.IsNullOrWhiteSpace(newServiceId.ToString()) || newServiceId.Equals(Guid.Empty))
            throw new DomainException("Serviço inválido.");
        ServiceId = newServiceId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }
    
    public void ChangeClient(Guid newClientId, Guid actor)
    {
        if (string.IsNullOrWhiteSpace(newClientId.ToString()) || newClientId.Equals(Guid.Empty))
            throw new DomainException("Cliente inválido.");
        ClientId = newClientId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Appends a note to the existing notes and updates the actor and timestamp information.
    /// </summary>
    /// <param name="note">The note text to add. If the existing notes are not empty, the new note is appended on a new line.</param>
    /// <param name="actor">The unique identifier of the actor adding the note. Used to update the record of who last modified the notes.</param>
    public void AddNotes(string? note, Guid actor)
    {
        if (string.IsNullOrWhiteSpace(note)) return;

        Notes = string.IsNullOrWhiteSpace(Notes) ? note : $"{Notes}\n{note}";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Extends the due date to a new specified value and records the actor responsible for the change.
    /// </summary>
    /// <param name="newDueDate">The new due date to set. Must be later than the current due date.</param>
    /// <param name="actor">The unique identifier of the user or process performing the update.</param>
    /// <exception cref="DomainException">Thrown if the specified new due date is not later than the current due date.</exception>
    public void ExtendDueDate(DateTime newDueDate, Guid actor)
    {
        if (newDueDate <= DueDate) throw new DomainException("Nova data de vencimento deve ser posterior à atual.");
        DueDate = newDueDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public DocumentDTO ToDTO()
    {
        return new DocumentDTO(
            Id,
            FileName,
            ClientId,
            ServiceId,
            ResponsibleId,
            IssueDate,
            DueDate,
            Notes,
            CreatedAt,
            CreatedBy,
            UpdatedAt,
            UpdatedBy,
            Status
        );
    }

    public DocumentWithMetadataDTO ToDTO(FileMetadataDTO? fileMetadata)
    {
        return new DocumentWithMetadataDTO(
            Id,
            FileName,
            ClientId,
            ServiceId,
            ResponsibleId,
            IssueDate,
            DueDate,
            Notes,
            CreatedAt,
            CreatedBy,
            UpdatedAt,
            UpdatedBy,
            Status,
            Client.Partner.LegalName,
            Client.LegalName,
            Service.Name,
            Responsible.Name,
            fileMetadata
        );
    }
}