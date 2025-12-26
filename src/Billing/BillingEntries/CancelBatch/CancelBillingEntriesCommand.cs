using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BillingEntries.CancelBatch;

public class CancelBillingEntriesCommand
{
    [Required]
    [MinLength(1)]
    public List<Guid> Ids { get; set; } = new();
}
