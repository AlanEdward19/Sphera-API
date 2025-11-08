namespace Sphera.API.Users.DTOs;

public class UserDTO
{
    public Guid Id { get; private set; }
    public short RoleId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public bool IsFirstAccess { get; private set; }
    public bool Active { get; private set; }

    public UserDTO(Guid id, short roleId, string name, string email, bool isFirstAccess, bool isActive)
    {
        Id = id;
        RoleId = roleId;
        Name = name;
        Email = email;
        IsFirstAccess = isFirstAccess;
        Active = isActive;
    }
}