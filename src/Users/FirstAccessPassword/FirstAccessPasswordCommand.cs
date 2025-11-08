using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Users.FirstAccessPassword;

public class FirstAccessPasswordCommand
{
    public Guid Id { get; private set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    public void SetId(Guid id) => Id = id;
}
