using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToPartner;

public record AddContactToPartnerCommand(Guid PartnerId, EContactType type, EContactRole role, string value);