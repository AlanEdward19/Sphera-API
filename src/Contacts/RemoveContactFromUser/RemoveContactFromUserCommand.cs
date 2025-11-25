namespace Sphera.API.Contacts.RemoveContactFromUser;

public record RemoveContactFromUserCommand(Guid UserId, Guid ContactId);