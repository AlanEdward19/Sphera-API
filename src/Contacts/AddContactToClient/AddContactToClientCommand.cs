using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToClient;

public class AddContactToClientCommand
{
    public Guid ClientId { get; private set; }
    public EContactType Type { get; set; }
    public EContactRole Role { get; set; }
    public string Value { get; set; }

    /// <summary>
    /// Sets the client identifier to the specified value.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the client.</param>
    public void SetClientId(Guid id) => ClientId = id;
}