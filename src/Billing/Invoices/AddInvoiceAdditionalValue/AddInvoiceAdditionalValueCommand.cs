using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.Invoices.AddInvoiceAdditionalValue;

public class AddInvoiceAdditionalValueCommand
{
    private Guid InvoiceId { get; set; }

    public void SetInvoiceId(Guid invoiceId) => InvoiceId = invoiceId;
    public Guid GetInvoiceId() => InvoiceId;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}