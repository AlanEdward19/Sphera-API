namespace Sphera.API.Billing.BilletConfigurations.DTOs;


public class BilletConfigurationDTO
{
    public Guid Id { get; set; }
    public string? CompanyCode { get; set; }
    public string? CompanyName { get; set; }
    public string? WalletNumber { get; set; }
    public string? AgencyNumber { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountDigit { get; set; }
    public string BankCode { get; set; } = "237";
    public bool HasFine { get; set; }
    public decimal? FinePercentage { get; set; }
    public decimal DailyDiscount { get; set; }
    public decimal DailyInterest { get; set; }
    public DateTime DiscountLimitDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RebateAmount { get; set; }
    public string? FirstMessage { get; set; }
    public string? SecondMessage { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static BilletConfigurationDTO FromEntity(BilletConfiguration entity)
    {
        return new BilletConfigurationDTO
        {
            Id = entity.Id,
            CompanyCode = entity.CompanyCode,
            CompanyName = entity.CompanyName,
            WalletNumber = entity.WalletNumber,
            AgencyNumber = entity.AgencyNumber,
            AccountNumber = entity.AccountNumber,
            AccountDigit = entity.AccountDigit,
            BankCode = entity.BankCode,
            HasFine = entity.HasFine,
            FinePercentage = entity.FinePercentage,
            DailyInterest = entity.DailyInterest,
            DiscountLimitDate = entity.DiscountLimitDate,
            DailyDiscount = entity.DailyDiscount,
            DiscountAmount = entity.DiscountAmount,
            RebateAmount = entity.RebateAmount,
            FirstMessage = entity.FirstMessage,
            SecondMessage = entity.SecondMessage,
            
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
