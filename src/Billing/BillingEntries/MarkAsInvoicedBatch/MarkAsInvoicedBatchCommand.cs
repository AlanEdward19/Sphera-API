using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BillingEntries.MarkAsInvoicedBatch;

public class MarkAsInvoicedBatchCommand
{
    [Required]
    [MinLength(1)]
    public List<Guid> Ids { get; set; } = new();

    [Required]
    public Guid InvoiceId { get; set; }
}
