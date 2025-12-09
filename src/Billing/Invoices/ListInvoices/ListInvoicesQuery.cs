using Sphera.API.Billing.Invoices.Enums;

namespace Sphera.API.Billing.Invoices.ListInvoices;

public class ListInvoicesQuery
{
    public Guid? ClientId { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public EInvoiceStatus? Status { get; set; }
}