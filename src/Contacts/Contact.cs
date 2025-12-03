using Newtonsoft.Json.Linq;
using Sphera.API.Clients;
using Sphera.API.Contacts.Enums;
using Sphera.API.Partners;
using Sphera.API.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Sphera.API.Users;

namespace Sphera.API.Contacts;

/// <summary>
/// Represents a contact entity with type, role, value, and audit information.
/// </summary>
/// <remarks>The Contact class encapsulates information about a contact, including its type, role, associated
/// value, and metadata for creation and updates. It is typically used to store and manage contact details within a
/// system, supporting concurrency control and audit tracking. Instances of this class are generally created and
/// modified through its public constructor and update methods, which enforce required fields and validation.</remarks>
public class Contact
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the associated partner, if available.
    /// </summary>
    public Guid? PartnerId { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the client associated with this instance, if available.
    /// </summary>
    public Guid? ClientId { get; private set; }
    
    public Guid? UserId { get; private set; }
    
    /// <summary>
    /// Gets or sets the name associated with the contact.
    /// </summary>
    [MinLength(1)]
    [MaxLength(160)]
    public string? Name { get; private set; }

    /// <summary>
    /// Gets or sets the type of contact represented by this instance.
    /// </summary>
    public EContactType Type { get; private set; }

    /// <summary>
    /// Gets or sets the role associated with the contact.
    /// </summary>
    public EContactRole Role { get; private set; }
    
    public EPhoneType? PhoneType { get; private set; }

    /// <summary>
    /// Gets or sets the string value associated with this instance.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who created the entity.
    /// </summary>
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; private set; }

    /// <summary>
    /// Gets or sets the row version for concurrency control.
    /// </summary>
    public byte[] RowVersion { get; private set; }

    /// <summary>
    /// Gets the partner entity associated with this instance.
    /// </summary>
    /// <remarks>This property is populated based on the foreign key relationship defined by the PartnerId
    /// property. The value may be null if no partner is associated.</remarks>
    [ForeignKey(nameof(PartnerId))]
    public virtual Partner? Partner { get; private set; }

    /// <summary>
    /// Gets the client associated with this entity.
    /// </summary>
    /// <remarks>This property is populated based on the foreign key relationship defined by the ClientId
    /// property. The value may be null if no client is associated. This property is read-only and typically set by the
    /// data access framework when loading related entities.</remarks>
    [ForeignKey(nameof(ClientId))]
    public virtual Client? Client { get; private set; }
    
    /// <summary>
    /// Gets the user associated with this contact.
    /// </summary>
    /// <remarks>This property is populated based on the foreign key relationship defined by the UserId
    /// property. The value may be null if no user is associated. This property is read-only and typically set by the
    /// data access framework when loading related entities.</remarks>
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Contact class. This constructor is private and is intended to restrict
    /// instantiation to within the class itself.
    /// </summary>
    private Contact()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Contact class with the specified contact type, role, value, and creator
    /// identifier.
    /// </summary>
    /// <param name="type">The type of contact to create. Cannot be null.</param>
    /// <param name="role">The role associated with the contact. Cannot be null.</param>
    /// <param name="value">The value of the contact, such as an email address or phone number. Cannot be null, empty, or consist only of
    /// white-space characters.</param>
    /// <param name="createdBy">The unique identifier of the user who created the contact.</param>
    /// <param name="clientId"></param>
    /// <param name="name"></param>
    /// <param name="partnerId"></param>
    /// <param name="userId"></param>
    /// <param name="phoneType"></param>
    /// <exception cref="DomainException">Thrown if type or role is null, or if value is null, empty, or consists only of white-space characters.</exception>
    public Contact(EContactType? type, EContactRole? role, string @value, Guid createdBy, Guid? partnerId = null,
        Guid? clientId = null, string? name = null, Guid? userId = null, EPhoneType? phoneType = null)
    {
        if (type == null) throw new DomainException("Type é obrigatório");
        if (role == null) throw new DomainException("Role é obrigatório");
        if (string.IsNullOrWhiteSpace(@value)) throw new DomainException("Value é obrigatório");
        if (partnerId is null && clientId is null && userId is null)
            throw new DomainException("O contato deve pertencer a um parceiro ou cliente.");

        @value = @value.Trim();

        ValidateValue(type.Value, @value);

        Id = Guid.NewGuid();
        Type = type.Value;
        Role = role.Value;
        Value = @value;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        PartnerId = partnerId;
        ClientId = clientId;
        Name = name;
        UserId = userId;
        PhoneType = phoneType;
    }

    /// <summary>
    /// Updates the current value and records the actor responsible for the change.
    /// </summary>
    /// <param name="newValue">The new value to assign. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="actor">The unique identifier of the actor performing the update.</param>
    /// <exception cref="DomainException">Thrown if newValue is null, empty, or consists only of white-space characters.</exception>
    public void UpdateValue(string? newValue, Guid actor)
    {
        if (string.IsNullOrWhiteSpace(newValue)) throw new DomainException("Value inválido");

        ValidateValue(Type, newValue);

        Value = newValue!;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Validates that the specified value conforms to the expected format for the given contact type.
    /// </summary>
    /// <param name="type">The type of contact to validate. Determines the expected format of the value.</param>
    /// <param name="value">The value to validate. The format must match the requirements for the specified contact type.</param>
    /// <exception cref="DomainException">Thrown if the value does not match the expected format for the specified contact type, or if the contact type is
    /// not supported.</exception>
    private void ValidateValue(EContactType type, string @value)
    {
        switch (type)
        {
            case EContactType.Email:
                Regex regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                if (!regex.IsMatch(@value))
                    throw new DomainException("Email inválido.");
                break;
            case EContactType.Phone:
                // Additional phone format validation can be added here if needed
                break;
            default:
                throw new DomainException("Tipo de contato inválido.");
        }
    }

    /// <summary>
    /// Updates the role associated with the contact and records the actor performing the update.
    /// </summary>
    /// <param name="newRole">The new role to assign to the contact. Must not be null.</param>
    /// <param name="actor">The unique identifier of the user or process performing the update.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="newRole"/> is <see langword="null"/>.</exception>
    public void UpdateRole(EContactRole? newRole, Guid actor)
    {
        if (newRole == null) throw new DomainException("Role inválido");
        Role = newRole.Value;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }
    
    public void UpdatePhoneType(EPhoneType? newPhoneType, Guid actor)
    {
        PhoneType = newPhoneType;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Updates the contact type and records the user responsible for the change.
    /// </summary>
    /// <param name="newType">The new contact type to assign. Cannot be null.</param>
    /// <param name="actor">The unique identifier of the user performing the update.</param>
    /// <exception cref="DomainException">Thrown if <paramref name="newType"/> is <see langword="null"/>.</exception>
    public void UpdateType(EContactType? newType, Guid actor)
    {
        if (newType == null) throw new DomainException("Type inválido");
        Type = newType.Value;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }
    
    public void UpdateName(string? newName, Guid actor)
    {
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    /// <summary>
    /// Converts the current contact to a data transfer object (DTO) representation.
    /// </summary>
    /// <returns>A <see cref="ContactDTO"/> instance containing the data from this contact.</returns>
    public ContactDTO ToDTO()
    {
        return new ContactDTO(Id, Name, Type, Role, PhoneType, Value, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);
    }
}