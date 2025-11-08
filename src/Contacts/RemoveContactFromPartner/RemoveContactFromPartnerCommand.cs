namespace Sphera.API.Contacts.RemoveContactFromPartner;

public record RemoveContactFromPartnerCommand(Guid PartnerId, Guid ContactId);