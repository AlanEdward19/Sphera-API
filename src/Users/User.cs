using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Auditory;
using Sphera.API.Contacts;
using Sphera.API.Documents;
using Sphera.API.Roles;
using Sphera.API.Shared.Services;
using Sphera.API.Shared.ValueObjects;
using Sphera.API.Users.CreateUser;
using Sphera.API.Users.DTOs;

namespace Sphera.API.Users;

public class User
{
    [Key] public Guid Id { get; private set; }

    [Required] public short RoleId { get; private set; }

    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string Name { get; private set; }

    [Required] public EmailValueObject Email { get; private set; }

    [Required] public PasswordValueObject Password { get; private set; }

    [Required] public bool Active { get; private set; }

    [Required] public bool IsFirstAccess { get; private set; }

    [Required] public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    [ForeignKey(nameof(RoleId))] public virtual Role Role { get; private set; }

    /// <summary>
    /// Gets the collection of contacts associated with this entity.
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; private set; } = new List<Contact>();

    /// <summary>
    /// Gets the collection of documents for which this user is the responsible party.
    /// </summary>
    public virtual ICollection<Document> Documents { get; private set; } = new List<Document>();

    /// <summary>
    /// Gets the collection of audit entries associated with this user as the actor.
    /// </summary>
    public virtual ICollection<AuditEntry> AuditEntries { get; private set; } = new List<AuditEntry>();

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

    public User(CreateUserCommand command)
    {
        {
            Id = Guid.NewGuid();
            RoleId = command.RoleId;
            Name = command.Name;
            Email = new EmailValueObject(command.Email);
            Password = new PasswordValueObject(PasswordGenerator.Generate());
            Active = true;
            IsFirstAccess = true;
            CreatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PasswordHash(string hashedPassword)
    {
        Password = new PasswordValueObject(hashedPassword, true);
    }

    public void ChangePassword(string newPassword)
    {
        Password = new PasswordValueObject(newPassword);
        IsFirstAccess = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(short roleId)
    {
        RoleId = roleId;
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

    public UserDTO ToDTO()
    {
        return new UserDTO(
            Id,
            RoleId,
            Name,
            Email.Address,
            IsFirstAccess,
            Active,
            Contacts.Select(c => c.ToDTO()).ToList().AsReadOnly()
        );
    }

    public bool CheckFirstAccess()
    {
        return IsFirstAccess;
    }
}