namespace Sphera.API.Contacts.RemoveContactFromClient;

public record RemoveContactFromClientCommand(Guid ClientId, Guid ContactId);