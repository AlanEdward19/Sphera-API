namespace Sphera.API.Billing.ClientServicePrices.ListClientServicePrices;

public class ListClientServicePricesQuery
{
    public Guid? ClientId { get; set; }
    public Guid? ServiceId { get; set; }
    public bool OnlyActive { get; set; } = true;
}