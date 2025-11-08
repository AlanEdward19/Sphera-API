using Sphera.API.Clients;
using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToPartner;

public class AddContactToPartnerCommand
{
    private Guid PartnerId { get; set; }
    public EContactType Type { get; set; }
    public EContactRole Role { get; set; }
    public string Value { get; set; }

    /// <summary>
    /// Gets the unique identifier of the partner associated with this instance.
    /// </summary>
    /// <returns>A <see cref="Guid"/> representing the partner's unique identifier.</returns>
    public Guid GetPartnerId() => PartnerId;

    /// <summary>
    /// Sets the partner identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign as the partner ID.</param>
    public void SetPartnerId(Guid id) => PartnerId = id;
}