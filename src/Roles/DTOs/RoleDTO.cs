namespace Sphera.API.Roles.DTOs;

public class RoleDTO(short id, string name)
{
    public short Id { get; } = id;
    public string Name { get; } = name;
}