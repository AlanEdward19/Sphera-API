using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Roles;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Users;

public class User
{
    [Key] 
    public Guid Id { get; private set; }

    [Required] 
    public int RoleId { get; private set; }

    [Required] 
    public string Name { get; private set; }

    [Required]
    public EmailValueObject Email { get; private set; }

    [Required] 
    public string Password { get; private set; }

    public bool Active { get; private set; }
    public bool IsFirstAccess { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    [ForeignKey(nameof(RoleId))] 
    public virtual Role Role { get; private set; }

    protected User()
    {
    }

    public User(int roleId, string name, EmailValueObject email, string password)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        Name = name;
        Email = email;
        Password = password;
        Active = true;
        IsFirstAccess = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(EmailValueObject email)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
        IsFirstAccess = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string password)
    {
        Password = password;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Active = true;
        UpdatedAt = DateTime.UtcNow;
    }
}