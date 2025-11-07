using Sphera.API.Clients;
using Sphera.API.Contacts;
using Sphera.API.Contacts.Enums;
using Sphera.API.Partners.CreatePartner;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared;
using Sphera.API.Shared.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Partners;

/// <summary>
/// Represents a business partner entity, including identification, registration, address, billing, and contact
/// information.
/// </summary>
/// <remarks>The Partner class encapsulates core data for a business partner, such as trade and legal names, CNPJ,
/// address, and registration details. It also manages the partner's activation status, billing due day, and associated
/// contacts. Use this class to create, update, activate, deactivate, and manage contacts for a partner within the
/// system. All properties are read-only outside the class, ensuring that changes to partner data are controlled through
/// explicit methods. Thread safety is not guaranteed; synchronize access if used concurrently.</remarks>
public class Partner
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the registered legal name of the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string LegalName { get; private set; }

    /// <summary>
    /// Gets the CNPJ (Cadastro Nacional da Pessoa Jurídica) identifier associated with this entity.
    /// </summary>
    /// <remarks>The CNPJ is a unique identifier assigned to legal entities in Brazil. Use this property to
    /// access the validated CNPJ value for the entity.</remarks>
    [Required]
    public CnpjValueObject? Cnpj { get; private set; }

    /// <summary>
    /// Gets the address associated with the current entity.
    /// </summary>
    [Required]
    public AddressValueObject? Address { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the current operation or entity is in an active or successful state.
    /// </summary>
    [Required]
    public bool Status { get; private set; }

    /// <summary>
    /// Gets the date and time when the object was created.
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
    /// scenarios. The value is updated automatically by the data store when the row is modified.</remarks>
    public byte[] RowVersion { get; private set; }

    /// <summary>
    /// Gets the collection of clients associated with this entity.
    /// </summary>
    /// <remarks>The returned collection is read-only and cannot be replaced, but clients can be added to or
    /// removed from the collection. Changes to the collection are reflected in the entity's client
    /// relationships.</remarks>
    public virtual ICollection<Client> Clients { get; private set; } = new List<Client>();

    /// <summary>
    /// Gets the collection of contacts associated with this entity.
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; private set; } = new List<Contact>();

    /// <summary>
    /// EF Core parameterless constructor.
    /// </summary>
    private Partner() { }

    /// <summary>
    /// Initializes a new instance of the Partner class with the specified legal name, CNPJ, address, and creator
    /// identifier.
    /// </summary>
    /// <param name="legalName">The legal name of the partner. Cannot be null or empty.</param>
    /// <param name="cnpj">The CNPJ (Cadastro Nacional da Pessoa Jurídica) identifier for the partner, or null if not applicable. Must be a
    /// valid CNPJ format if provided.</param>
    /// <param name="address">The address information for the partner, or null if not specified.</param>
    /// <param name="createdBy">The unique identifier of the user who created the partner record.</param>
    public Partner(
        string legalName,
        string? cnpj,
        AddressValueObject? address,
        Guid createdBy)
    {
        Id = Guid.NewGuid();
        CnpjValueObject? cnpjVo = string.IsNullOrWhiteSpace(cnpj) ? null : new(cnpj);

        SetBasicInfo(legalName, cnpjVo, address);
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Status = true;
    }

    /// <summary>
    /// Initializes a new instance of the Partner class using the specified partner creation command and creator
    /// identifier.
    /// </summary>
    /// <param name="command">The command containing the legal name, CNPJ, and address information required to create the partner. Cannot be
    /// null.</param>
    /// <param name="createdBy">The unique identifier of the user or process that created the partner.</param>
    public Partner(CreatePartnerCommand command, Guid createdBy)
    {
        Id = Guid.NewGuid();

        CnpjValueObject? cnpj = string.IsNullOrWhiteSpace(command.Cnpj) ? null : new(command.Cnpj);

        SetBasicInfo(command.LegalName, cnpj, command.Address?.ToValueObject());
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Status = true;
    }

    /// <summary>
    /// Sets the basic company information, including trade name, legal name, CNPJ, address, and optional billing due
    /// day.
    /// </summary>
    /// <param name="tradeName">The trade name of the company. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="legalName">The legal name of the company. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="cnpj">The CNPJ value object representing the company's registration number. Cannot be null.</param>
    /// <param name="address">The address value object representing the company's location. Cannot be null.</param>
    /// <param name="billingDueDay">The day of the month on which billing is due. Optional; may be null if not applicable.</param>
    /// <exception cref="DomainException">Thrown if any required parameter is null, empty, or invalid.</exception>
    public void SetBasicInfo(
        string legalName,
        CnpjValueObject? cnpj,
        AddressValueObject? address)
    {
        if (string.IsNullOrWhiteSpace(legalName))
            throw new DomainException("Razão social obrigatória.");

        LegalName = legalName;
        Cnpj = cnpj;
        Address = address;
    }

    /// <summary>
    /// Updates the legal name, CNPJ, and address information for the entity, and records the actor responsible for the
    /// update.
    /// </summary>
    /// <param name="legalName">The new legal name to assign to the entity. Cannot be null or empty.</param>
    /// <param name="cnpj">The CNPJ value to associate with the entity, or null to clear the existing CNPJ.</param>
    /// <param name="address">The address information to assign to the entity, or null to clear the existing address.</param>
    /// <param name="actorId">The unique identifier of the actor performing the update. Used to track who made the change.</param>
    public void UpdateBasicInfo(
        string legalName,
        CnpjValueObject? cnpj,
        AddressValueObject? address,
        Guid actorId)
    {
        SetBasicInfo(legalName, cnpj, address);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actorId;
    }

    /// <summary>
    /// Sets the entity's status to active.
    /// </summary>
    /// <param name="actorId"></param>
    public void Activate(Guid actorId)
    {
        Status = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actorId;
    }

    /// <summary>
    /// Sets the entity's status to inactive.
    /// </summary>
    /// <param name="actorId"></param>
    public void Deactivate(Guid actorId)
    {
        Status = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actorId;
    }

    /// <summary>
    /// Adds a new contact with the specified type, role, value, and associated actor to the collection of contacts.
    /// </summary>
    /// <param name="contactType">The type of contact to add. Specifies how the contact information is categorized (for example, email or phone).</param>
    /// <param name="contactRole">The role associated with the contact. Determines the contact's function or relationship within the context.</param>
    /// <param name="value">The contact information value, such as an email address or phone number. Cannot be null.</param>
    /// <param name="actorId">The unique identifier of the actor to associate with the new contact.</param>
    /// <returns>The newly created Contact instance that was added to the collection.</returns>
    public Contact AddContact(
        EContactType contactType,
        EContactRole contactRole,
        string value,
        Guid actorId)
    {
        var contact = new Contact(contactType, contactRole, value, actorId, Id);
        Contacts.Add(contact);
        return contact;
    }

    /// <summary>
    /// Removes the contact with the specified unique identifier from the collection of contacts.
    /// </summary>
    /// <remarks>If no contact with the specified identifier exists in the collection, the method performs no
    /// action.</remarks>
    /// <param name="contactId">The unique identifier of the contact to remove from the collection.</param>
    public void RemoveContact(Guid contactId)
    {
        var contact = Contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is not null)
            Contacts.Remove(contact);
    }

    /// <summary>
    /// Converts the current partner entity to a data transfer object (DTO) representation.
    /// </summary>
    /// <param name="includeClients">Indicates whether to include associated client information in the returned DTO. Specify <see langword="true"/>
    /// to include clients; otherwise, only partner details are included.</param>
    /// <returns>A <see cref="PartnerDTO"/> instance representing the partner. If <paramref name="includeClients"/> is <see
    /// langword="true"/>, the returned object includes client data; otherwise, it does not.</returns>
    public PartnerDTO ToDTO(bool includeClients)
    {
        return includeClients
            ? new PartnerWithClientsDTO(Id, LegalName, Cnpj.Value, Address.ToDTO(), Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
                Contacts.Select(c => c.ToDTO()).ToList().AsReadOnly(),
                Clients.Select(c => c.ToDTO(includePartner: false)).ToList().AsReadOnly())
            : new PartnerDTO(Id, LegalName, Cnpj.Value, Address.ToDTO(), Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy,
                Contacts.Select(c => c.ToDTO()).ToList().AsReadOnly());
    }
}
