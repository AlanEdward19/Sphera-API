using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.EditContact;

/// <summary>
/// Represents a command to edit the details of a contact, including its value, type, and role.
/// </summary>
public class EditContactCommand
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    private Guid Id { get; set; }
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the string value associated with this instance.
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Gets or sets the type of contact information represented by this instance.
    /// </summary>
    public EContactType? Type { get; set; }
    
    /// <summary>
    /// Gets or sets the role associated with the contact, such as manager, assistant, or primary contact.
    /// </summary>
    public EContactRole? Role { get; set; }
    
    public EPhoneType? PhoneType { get; set; }

    /// <summary>
    /// Retrieves the current identifier value.
    /// </summary>
    public Guid GetId() => Id;

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => Id = id;
}