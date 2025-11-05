using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Roles;

public class Role
{
    [Key]
    public short Id { get; private set; }

    [Required]
    public string Name { get; private set; }
    
    [Required]
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? UpdatedAt { get; private set; }
    
    protected Role() {} 
    
    public Role(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}