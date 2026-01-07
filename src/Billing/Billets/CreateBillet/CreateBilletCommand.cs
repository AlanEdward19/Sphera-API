using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.Billets.CreateBillet;

public class CreateBilletCommand
{
    [Required]
    public Guid InstallmentId { get; set; }

    [Required]
    public Guid ConfigurationId { get; set; }

    [Required]
    public Guid ClientId { get; set; }
}

