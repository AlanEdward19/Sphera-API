using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Users.UpdateUser;

/// <summary>
/// Command to update basic information about a user.
/// </summary>
public class UpdateUserCommand
{
    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the role identifier of the user.
    /// </summary>
    [Range(1, 3)]
    public short? RoleId { get; set; }

    /// <summary>
    /// Sets the identifier for this command instance.
    /// </summary>
    /// <param name="id">The user id.</param>
    public void SetId(Guid id) => Id = id;
}
