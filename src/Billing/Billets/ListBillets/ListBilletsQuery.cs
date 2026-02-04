namespace Sphera.API.Billing.Billets.ListBillets;

public class ListBilletsQuery
{
    public Guid? ClientId { get; set; }
    public Guid? InstallmentId { get; set; }
}

