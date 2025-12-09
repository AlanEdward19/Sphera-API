using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BillingEntries.CreateBillingEntry;

public class CreateBillingEntryCommand
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    public DateTime ServiceDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}