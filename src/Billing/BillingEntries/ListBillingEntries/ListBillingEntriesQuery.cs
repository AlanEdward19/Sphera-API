namespace Sphera.API.Billing.BillingEntries.ListBillingEntries;

public class ListBillingEntriesQuery
{
    public Guid? ClientId { get; set; }
    public DateTime? ServiceDateStart { get; set; }
    public DateTime? ServiceDateEnd { get; set; }
    public bool? IsBillable { get; set; }
    public bool OnlyWithoutInvoice { get; set; } = false;
}