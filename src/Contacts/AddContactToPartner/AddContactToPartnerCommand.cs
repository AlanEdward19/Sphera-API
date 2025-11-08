using Sphera.API.Clients;
using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToPartner;

public class AddContactToPartnerCommand
{
    public Guid PartnerId { get; private set; }
    public EContactType Type { get; set; }
    public EContactRole Role { get; set; }
    public string Value { get; set; }

    /// <summary>
    /// Sets the partner identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign as the partner ID.</param>
    public void SetPartnerId(Guid id) => PartnerId = id;
}