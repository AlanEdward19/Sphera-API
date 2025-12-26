using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.ClientServicePrices.CreateClientServicePrice;

public class CreateClientServicePriceCommand
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    public DateTime StartDate { get; set; }
}