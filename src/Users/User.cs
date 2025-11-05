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
    public short RoleId { get; private set; }

    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string Name { get; private set; }

    [Required]
    public EmailValueObject Email { get; private set; }

    [Required]
    public PasswordValueObject Password { get; private set; }

    [Required]
    public bool Active { get; private set; }
    
    [Required]
    public bool IsFirstAccess { get; private set; }

    [Required] 
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? UpdatedAt { get; private set; }

    [ForeignKey(nameof(RoleId))] 
    public virtual Role Role { get; private set; }

    protected User()
    {
    }

    public User(short roleId, string name, EmailValueObject email, PasswordValueObject password)
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

    public void ChangePassword(PasswordValueObject newPassword)
    {
        Password = newPassword;
        IsFirstAccess = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(PasswordValueObject password)
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