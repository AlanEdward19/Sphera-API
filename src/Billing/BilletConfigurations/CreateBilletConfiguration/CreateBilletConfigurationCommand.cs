using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.BilletConfigurations.CreateBilletConfiguration;

public class CreateBilletConfigurationCommand
{
    [Required]
    [MaxLength(20)]
    public string CompanyCode { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string CompanyName { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string WalletNumber { get; set; }
    
    [Required]
    [MaxLength(5)]
    public string AgencyNumber { get; set; }
    
    [Required]
    [MaxLength(7)]
    public string AccountNumber { get; set; }
    
    [MaxLength(1)]
    public string? AccountDigit { get; set; }
    
    [MaxLength(3)]
    public string BankCode { get; set; } = "237";

    public bool HasFine { get; set; } = false;
    
    public decimal? FinePercentage { get; set; }
    
    public decimal DailyDiscount { get; set; }
    
    public decimal DailyInterest { get; set; }
    
    public DateTime DiscountLimitDate { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    public decimal RebateAmount { get; set; }
    
    [MaxLength(12)]
    public string? FirstMessage { get; set; }
    
    [MaxLength(60)]
    public string? SecondMessage { get; set; }
    
    public int StartingSequentialNumber { get; set; }
    
    public int StartingNossoNumero { get; set; }
}

