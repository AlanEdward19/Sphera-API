using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BillingEntries.UpdateBillingEntry;

public class UpdateBillingEntryCommand
{
    private Guid Id { get; set; }

    public void SetId(Guid id) => Id = id; 
    public Guid GetId() => Id;

    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    public string? Notes { get; set; }

    [Required]
    public bool IsBillable { get; set; }
}