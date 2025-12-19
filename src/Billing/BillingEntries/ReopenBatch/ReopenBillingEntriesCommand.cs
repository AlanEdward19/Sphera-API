using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BillingEntries.ReopenBatch;

public class ReopenBillingEntriesCommand
{
    [Required]
    [MinLength(1)]
    public List<Guid> Ids { get; set; } = new();
}
