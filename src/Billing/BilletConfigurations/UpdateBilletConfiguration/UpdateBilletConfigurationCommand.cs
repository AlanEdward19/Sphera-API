using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BilletConfigurations.UpdateBilletConfiguration;

public class UpdateBilletConfigurationCommand
{
    private Guid Id { get; set; }
    public void SetId(Guid id) => Id = id;
    public Guid GetId() => Id;

    [MaxLength(20)]
    public string? CompanyCode { get; set; }
    [MaxLength(30)]
    public string? CompanyName { get; set; }
    [MaxLength(3)]
    public string? WalletNumber { get; set; }
    [MaxLength(5)]
    public string? AgencyNumber { get; set; }
    [MaxLength(7)]
    public string? AccountNumber { get; set; }
    [MaxLength(1)]
    public string? AccountDigit { get; set; }
    [MaxLength(3)]
    public string? BankCode { get; set; }

    public bool? HasFine { get; set; }
    public decimal? FinePercentage { get; set; }
    public decimal? DailyDiscount { get; set; }
    public decimal? DailyInterest { get; set; }
    public DateTime? DiscountLimitDate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? RebateAmount { get; set; }
    [MaxLength(12)]
    public string? FirstMessage { get; set; }
    [MaxLength(60)]
    public string? SecondMessage { get; set; }
    public int? StartingSequentialNumber { get; set; }
}

