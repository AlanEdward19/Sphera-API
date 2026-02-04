using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.Remittances.CreateRemittance;

public class CreateRemittanceCommand
{
    [Required]
    public IReadOnlyCollection<Guid> BilletIds { get; set; } = Array.Empty<Guid>();

    public string FileName { get; set; } = string.Empty;
}



