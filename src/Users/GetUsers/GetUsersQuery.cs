namespace Sphera.API.Users.GetUsers;

public class GetUsersQuery
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    public int? RoleId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}