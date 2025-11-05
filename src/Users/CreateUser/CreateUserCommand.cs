using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Users.CreateUser;

public class CreateUserCommand
{
    /// <summary>
    /// Get or sets the name of the user.
    /// </summary>
    [Required]
    public string Name { get; set; }
    
    /// <summary>
    /// Get or sets the email of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    /// <summary>
    /// Get or sets the password of the user.
    /// </summary>
    [Required]
    [Range(1, 3)]
    public short RoleId { get; set; }
}