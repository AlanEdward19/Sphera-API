using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToClient;

public class AddContactToClientCommand
{
    private Guid ClientId { get; set; }

    public EContactType Type { get; set; }
    public EContactRole Role { get; set; }
    public string Value { get; set; }

    /// <summary>
    /// Gets the unique identifier associated with the client.
    /// </summary>
    /// <returns>A <see cref="Guid"/> representing the client's unique identifier.</returns>
    public Guid GetClientId() => ClientId;
    
    /// <summary>
    /// Sets the client identifier to the specified value.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the client.</param>
    public void SetClientId(Guid id) => ClientId = id;
}