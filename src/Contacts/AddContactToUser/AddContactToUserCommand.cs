using Sphera.API.Contacts.Enums;

namespace Sphera.API.Contacts.AddContactToUser;

public class AddContactToUserCommand
{
    private Guid UserId { get; set; }
    public EContactType Type { get; set; }
    public EContactRole Role { get; set; }
    public EPhoneType? PhoneType { get; set; }
    public string Value { get; set; }
    
    public Guid GetUserId() => UserId;
    public void SetUserId(Guid id) => UserId = id;
}