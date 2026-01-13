using System.ComponentModel.DataAnnotations;
using Sphera.API.Billing.Billets.Enums;

namespace Sphera.API.Billing.Billets.CreateBillet;

public class CreateBilletCommand
{
    [Required]
    public EBilletBank Bank { get; set; }
    
    [Required]
    public Guid InstallmentId { get; set; }

    [Required]
    public Guid ConfigurationId { get; set; }

    [Required]
    public Guid ClientId { get; set; }
}

