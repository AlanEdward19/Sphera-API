using Sphera.API.Clients.CreateClient;
using Sphera.API.Clients.DTOs;
using Sphera.API.Contacts;
using Sphera.API.Contacts.Enums;
using Sphera.API.Partners;
using Sphera.API.Shared;
using Sphera.API.Shared.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphera.API.Clients;

public class Client
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the partner associated with this entity.
    /// </summary>
    [Required]
    public Guid PartnerId { get; private set; }

    /// <summary>
    /// Gets the trade name associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string TradeName { get; private set; }

    /// <summary>
    /// Gets the legal name associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string LegalName { get; private set; }

    /// <summary>
    /// Gets the CNPJ (Cadastro Nacional da Pessoa Jurídica) value associated with the entity.
    /// </summary>
    /// <remarks>The CNPJ is a unique identifier assigned to legal entities in Brazil. This property is
    /// required and cannot be null.</remarks>
    [Required]
    public CnpjValueObject Cnpj { get; private set; }

    /// <summary>
    /// Gets the state registration identifier associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string StateRegistration { get; private set; }

    /// <summary>
    /// Gets the municipal registration number associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string MunicipalRegistration { get; private set; }

    /// <summary>
    /// Gets the address associated with the entity.
    /// </summary>
    [Required]
    public AddressValueObject Address { get; private set; }

    /// <summary>
    /// Gets the day of the month on which billing is due, if specified.
    /// </summary>
    [Range(1, 31)]
    public short? BillingDueDay { get; private set; }
    
    /// <summary>
    /// Gets the date when the contract was established, if applicable.
    /// </summary>
    public DateTime? ContractDate { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the current operation or entity is in an active or successful state.
    /// </summary>
    [Required]
    public bool Status { get; private set; }

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
    /// scenarios. The value is updated automatically by the data store when the row is modified.</remarks>
    public byte[] RowVersion { get; private set; }

    /// <summary>
    /// Gets the partner entity associated with this instance.
    /// </summary>
    [ForeignKey(nameof(PartnerId))]
    public virtual Partner Partner { get; private set; }

    /// <summary>
    /// Gets the collection of contacts associated with this entity.
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; private set; } = new List<Contact>();

    /// <summary>
    /// EF Core parameterless constructor.
    /// </summary>
    private Client() { }

    /// <summary>
    /// Initializes a new instance of the Client class with the specified identifiers, names, CNPJ, address, creator,
    /// and optional billing due day.
    /// </summary>
    /// <param name="id">The unique identifier for the client. If Guid.Empty is provided, a new identifier is generated.</param>
    /// <param name="partnerId">The unique identifier of the associated partner. Cannot be Guid.Empty.</param>
    /// <param name="tradeName">The trade name of the client.</param>
    /// <param name="legalName">The legal name of the client.</param>
    /// <param name="cnpj">The CNPJ value object representing the client's tax identification number, or null if not applicable.</param>
    /// <param name="address">The address value object representing the client's address, or null if not provided.</param>
    /// <param name="createdBy">The unique identifier of the user who created the client.</param>
    /// <param name="contractDate"></param>
    /// <param name="billingDueDay">The day of the month when billing is due, or null if not specified.</param>
    /// <exception cref="DomainException">Thrown if partnerId is Guid.Empty.</exception>
    public Client(Guid partnerId, string tradeName, string legalName, CnpjValueObject? cnpj, string stateRegistration,
        string municipalRegistration, AddressValueObject? address, Guid createdBy, DateTime contractDate, short? billingDueDay = null)
    {
        Id = Guid.NewGuid();
        if (partnerId == Guid.Empty) throw new DomainException("PartnerId obrigatório.");
        PartnerId = partnerId;
        SetBasicInfo(tradeName, legalName, cnpj, stateRegistration, municipalRegistration, address, contractDate, billingDueDay);
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Status = true;
    }

    public Client(CreateClientCommand command, Guid createdBy)
    {
        Id = Guid.NewGuid();
        if (command.PartnerId == Guid.Empty) throw new DomainException("PartnerId obrigatório.");
        PartnerId = command.PartnerId;
        DateTime contractDate = DateTime.Today.AddDays(command.ContractDateInDays);
        SetBasicInfo(
            command.TradeName,
            command.LegalName,
            new CnpjValueObject(command.Cnpj),
            command.StateRegistration,
            command.MunicipalRegistration,
            command.Address.ToValueObject(),
            contractDate,
            command.BillingDueDay);
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Status = true;
    }

    /// <summary>
    /// Sets the basic information for the entity, including trade name, legal name, CNPJ, address, and optional billing
    /// due day.
    /// </summary>
    /// <param name="tradeName">The trade name to assign. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="legalName">The legal name to assign. May be null or empty if not required by the domain.</param>
    /// <param name="cnpj">The CNPJ value object representing the entity's CNPJ. Cannot be null.</param>
    /// <param name="stateRegistration"></param>
    /// <param name="municipalRegistration"></param>
    /// <param name="address">The address value object representing the entity's address. Cannot be null.</param>
    /// <param name="contractDate"></param>
    /// <param name="billingDueDay">The day of the month on which billing is due. May be null if not applicable.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="tradeName"/> is null, empty, or white space; or if <paramref name="cnpj"/> or
    /// <paramref name="address"/> is null.</exception>
    private void SetBasicInfo(string tradeName, string legalName, CnpjValueObject? cnpj, string stateRegistration,
        string municipalRegistration, AddressValueObject? address, DateTime contractDate,
       short? billingDueDay)
    {
        if (string.IsNullOrWhiteSpace(tradeName)) throw new DomainException("Nome fantasia obrigatório.");
        if (string.IsNullOrWhiteSpace(legalName)) throw new DomainException("Razão social obrigatória.");

        TradeName = tradeName;
        LegalName = legalName;
        Cnpj = cnpj ?? throw new DomainException("CNPJ obrigatório.");
        StateRegistration = stateRegistration;
        MunicipalRegistration = municipalRegistration;
        Address = address ?? throw new DomainException("Endereço obrigatório.");
        BillingDueDay = billingDueDay;
        ContractDate = contractDate;
    }

    /// <summary>
    /// Updates the basic company information, including trade name, legal name, CNPJ, address, and billing due day, and
    /// records the actor performing the update.
    /// </summary>
    /// <param name="tradeName">The trade name to assign to the company. Cannot be null or empty.</param>
    /// <param name="legalName">The legal name to assign to the company. Cannot be null or empty.</param>
    /// <param name="cnpj">The CNPJ value object representing the company's registration number. Can be null if not applicable.</param>
    /// <param name="address">The address value object representing the company's location. Can be null if not applicable.</param>
    /// <param name="contractDate"></param>
    /// <param name="billingDueDay">The day of the month when billing is due. Must be between 1 and 31, or null if not set.</param>
    /// <param name="actor">The unique identifier of the user or process performing the update.</param>
    public void UpdateBasicInfo(string tradeName, string legalName, CnpjValueObject? cnpj, string stateRegistration,
        string municipalRegistration, AddressValueObject? address, DateTime contractDate, short? billingDueDay, Guid actor)
    {
        SetBasicInfo(tradeName, legalName, cnpj, stateRegistration, municipalRegistration, address, contractDate, billingDueDay);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Activates the entity and records the user responsible for the activation.
    /// </summary>
    /// <param name="actor">The unique identifier of the user or process performing the activation. This value is recorded as the updater of
    /// the entity.</param>
    public void Activate(Guid actor)
    {
        Status = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Deactivates the current entity and records the actor responsible for the change.
    /// </summary>
    /// <param name="actor">The unique identifier of the actor performing the deactivation. This value is used to update the audit
    /// information.</param>
    public void Deactivate(Guid actor)
    {
        Status = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Adds a new contact with the specified type, role, value, and actor identifier.
    /// </summary>
    /// <param name="contactType">The type of contact to add. Specifies how the contact information is used (for example, email or phone).</param>
    /// <param name="contactRole">The role associated with the contact. Determines the contact's relationship or function.</param>
    /// <param name="value">The contact information value, such as an email address or phone number. Cannot be null.</param>
    /// <param name="actorId">The unique identifier of the actor associated with the contact.</param>
    /// <returns>The newly created Contact instance representing the added contact.</returns>
    public Contact AddContact(
        EContactType contactType,
        EContactRole contactRole,
        string value,
        Guid actorId)
    {
        var contact = new Contact(contactType, contactRole, value, actorId, null, Id);
        Contacts.Add(contact);
        return contact;
    }

    /// <summary>
    /// Removes the contact with the specified unique identifier from the collection of contacts, if it exists.
    /// </summary>
    /// <param name="contactId">The unique identifier of the contact to remove from the collection.</param>
    public void RemoveContact(Guid contactId)
    {
        var contact = Contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is not null)
            Contacts.Remove(contact);
    }

    public ClientDTO ToDTO(bool includePartner)
    {
        return includePartner ? new ClientWithPartnerDTO
        (
            Id,
            TradeName,
            LegalName,
            Cnpj.Value,
            StateRegistration,
            MunicipalRegistration,
            Address.ToDTO(),
            BillingDueDay,
            ContractDate,
            Status,
            CreatedAt,
            CreatedBy,
           UpdatedAt,
            UpdatedBy,
            Contacts.Select(c => c.ToDTO()).ToList().AsReadOnly(),
            Partner.ToDTO(false)
        ) : new ClientDTO(
            Id,
            TradeName,
            LegalName,
            Cnpj.Value,
            StateRegistration,
            MunicipalRegistration,
            Address.ToDTO(),
            BillingDueDay,
            ContractDate,
            Status,
            CreatedAt,
            CreatedBy,
           UpdatedAt,
            UpdatedBy,
             Contacts.Select(c => c.ToDTO()).ToList().AsReadOnly()
        );
    }
}