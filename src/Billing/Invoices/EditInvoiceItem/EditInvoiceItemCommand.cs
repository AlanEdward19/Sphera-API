using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.Invoices.EditInvoiceItem;

public class EditInvoiceItemCommand
{
    private Guid InvoiceId { get; set; }
    private Guid ItemId { get; set; }

    public void SetInvoiceId(Guid invoiceId) => InvoiceId = invoiceId;
    public Guid GetInvoiceId() => InvoiceId;

    public void SetItemId(Guid itemId) => ItemId = itemId;
    public Guid GetItemId() => ItemId;

    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}
