using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts;

/// <summary>
/// Represents a data transfer object for contact information, including type, role, value, and audit metadata.
/// </summary>
/// <param name="id">The unique identifier for the contact.</param>
/// <param name="type">The type of contact, such as email, phone, or address.</param>
/// <param name="role">The role associated with the contact, indicating its purpose or context.</param>
/// <param name="value">The value of the contact, such as an email address or phone number.</param>
/// <param name="createdAt">The date and time when the contact was created.</param>
/// <param name="createdBy">The unique identifier of the user who created the contact.</param>
/// <param name="updatedAt">The date and time when the contact was last updated, or null if it has not been updated.</param>
/// <param name="updatedBy">The unique identifier of the user who last updated the contact, or null if it has not been updated.</param>
public class ContactDTO(
    Guid id,
    EContactType type,
    EContactRole role,
    string value,
    DateTime createdAt,
    Guid createdBy,
    DateTime? updatedAt,
    Guid? updatedBy)
{
    /// <summary>
    /// Gets the unique identifier for the instance.
    /// </summary>
    public Guid Id { get; private set; } = id;

    /// <summary>
    /// Gets the type of contact represented by this instance.
    /// </summary>
    public EContactType Type { get; private set; } = type;

    /// <summary>
    /// Gets the role associated with the contact.
    /// </summary>
    public EContactRole Role { get; private set; } = role;

    /// <summary>
    /// Gets the current value as a string.
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Gets the date and time when the object was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = createdAt;

    /// <summary>
    /// Gets the unique identifier of the user who created the entity.
    /// </summary>
    public Guid CreatedBy { get; private set; } = createdBy;

    /// <summary>
    /// Gets the date and time when the entity was last updated, or null if it has not been updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; } = updatedAt;

    /// <summary>
    /// Gets the unique identifier of the user who last updated the entity, or null if it has not been updated.
    /// </summary>
    public Guid? UpdatedBy { get; private set; } = updatedBy;
}
