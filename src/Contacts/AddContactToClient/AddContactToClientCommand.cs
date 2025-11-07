using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToClient;

public class AddContactToClientCommand(Guid ClientId, EContactType type, EContactRole role, string value);