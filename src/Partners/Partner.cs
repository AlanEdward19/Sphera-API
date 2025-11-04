using Sphera.API.Contacts;
using Sphera.API.Contacts.Enums;
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
    /// Gets the trade name associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string TradeName { get; private set; }

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
    public CnpjValueObject Cnpj { get; private set; }

    /// <summary>
    /// Gets the state registration identifier associated with the entity.
    /// </summary>
    [MaxLength(50)]
    public string? StateRegistration { get; private set; }

    /// <summary>
    /// Gets the municipal registration number associated with the entity.
    /// </summary>
    [MaxLength(50)]
    public string? MunicipalRegistration { get; private set; }

    /// <summary>
    /// Gets the address associated with the current entity.
    /// </summary>
    [Required]
    public AddressValueObject Address { get; private set; }

    /// <summary>
    /// Gets the day of the month on which billing is due, if specified.
    /// </summary>
    [Range(1, 31)]
    public short? BillingDueDay { get; private set; }

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
    /// Gets the collection of contacts associated with this entity.
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; private set; } = new List<Contact>();

    /// <summary>
    /// EF Core parameterless constructor.
    /// </summary>
    private Partner() { }

    /// <summary>
    /// Initializes a new instance of the Partner class with the specified identification, business information, CNPJ,
    /// address, creator, and optional billing due day.
    /// </summary>
    /// <param name="id">The unique identifier for the partner. If Guid.Empty is provided, a new identifier is generated.</param>
    /// <param name="tradeName">The trade name of the partner. This represents the commonly used business name.</param>
    /// <param name="legalName">The legal name of the partner as registered with authorities.</param>
    /// <param name="cnpj">The CNPJ value object representing the partner's Brazilian company registration number.</param>
    /// <param name="address">The address value object containing the partner's location details.</param>
    /// <param name="createdBy">The unique identifier of the user or entity that created the partner record.</param>
    /// <param name="billingDueDay">The day of the month when billing is due for the partner. If null, no specific billing due day is set.</param>
    public Partner(
        Guid id,
        string tradeName,
        string legalName,
        CnpjValueObject cnpj,
        AddressValueObject address,
        Guid createdBy,
        short? billingDueDay = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        SetBasicInfo(tradeName, legalName, cnpj, address, billingDueDay);
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
        string tradeName,
        string legalName,
        CnpjValueObject cnpj,
        AddressValueObject address,
        short? billingDueDay)
    {
        if (string.IsNullOrWhiteSpace(tradeName))
            throw new DomainException("Nome fantasia obrigatório.");
        if (string.IsNullOrWhiteSpace(legalName))
            throw new DomainException("Razão social obrigatória.");

        TradeName = tradeName;
        LegalName = legalName;
        Cnpj = cnpj ?? throw new DomainException("CNPJ obrigatório.");
        Address = address ?? throw new DomainException("Endereço obrigatório.");
        BillingDueDay = billingDueDay;
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
}
