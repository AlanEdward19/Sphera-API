using System.ComponentModel.DataAnnotations;
using Sphera.API.Billing.BilletConfigurations.CreateBilletConfiguration;
using Sphera.API.Billing.BilletConfigurations.UpdateBilletConfiguration;
using Sphera.API.Billing.Billets;

namespace Sphera.API.Billing.BilletConfigurations;

public class BilletConfiguration
{
    [Key]
    public Guid Id { get; set; }
    
    // Arquivo de Remessa
    // 027 a 046 - Código da Empresa
    [MaxLength(20)]
    public string CompanyCode { get; set; }
    
    // 047 a 076 - Nome da Empresa
    [MaxLength(30)]
    public string CompanyName { get; set; }
    
    // Arquivo Tipo 1
    
    // 021 a 037 - Identificação da Empresa Beneficiária no Banco
    [MaxLength(3)]
    public string WalletNumber { get; set; }
    
    [MaxLength(5)]
    public string AgencyNumber { get; set; }
    
    [MaxLength(7)]
    public string AccountNumber { get; set; }
    
    [MaxLength(1)]
    public string AccountDigit { get; set; }
    
    // 063 a 065 - Código do Banco a ser debitado na Câmara de Compensação
    [MaxLength(3)]
    public string BankCode { get; set; } = "237";

    // 066 a 066 - Identificativos de Multa
    public bool HasFine { get; set; } = false;
    
    // 067 a 070 - Percentual de Multa por Atraso
    public decimal? FinePercentage { get; set; }
    
    // 083 a 092 - Desconto Bonificação por dia
    public decimal DailyDiscount { get; set; }
    
    // 161 a 173 - Valores a serem Cobrados por Dia de Atraso (Mora Dia)
    public decimal DailyInterest { get; set; }
    
    // 174 a 179 - Data Limite P/Concessão de Desconto
    public DateTime DiscountLimitDate { get; set; }
    
    // 180 a 192 - Valor do Desconto
    public decimal DiscountAmount { get; set; }
    
    // 206 a 218 - Valor do Abatimento a ser Concedido ou Cancelado
    public decimal RebateAmount { get; set; }
    
    // 315 a 326 - 1ª Mensagem
    [MaxLength(12)]
    public string FirstMessage { get; set; }
    
    // 335 a 394 - 2ª Mensagem
    [MaxLength(60)]
    public string SecondMessage { get; set; }
    
    public int StartingSequentialNumber { get; set; }
    
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who created the entity.
    /// </summary>
    [Required]
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; private set; }

    public virtual ICollection<Billet> Billets { get; set; } = new List<Billet>();

    public BilletConfiguration() { }

    public BilletConfiguration(string companyCode, string companyName, string walletNumber, string agencyNumber, string accountNumber, string accountDigit, string bankCode, bool hasFine, decimal? finePercentage, decimal dailyDiscount, decimal dailyInterest, DateTime discountLimitDate, decimal discountAmount, decimal rebateAmount, string firstMessage, string secondMessage, int startingSequentialNumber, Guid createdBy)
    {
        Id = Guid.NewGuid();
        CompanyCode = companyCode;
        CompanyName = companyName;
        WalletNumber = walletNumber;
        AgencyNumber = agencyNumber;
        AccountNumber = accountNumber;
        AccountDigit = accountDigit;
        BankCode = bankCode;
        HasFine = hasFine;
        FinePercentage = finePercentage;
        DailyDiscount = dailyDiscount;
        DailyInterest = dailyInterest;
        DiscountLimitDate = discountLimitDate;
        DiscountAmount = discountAmount;
        RebateAmount = rebateAmount;
        FirstMessage = firstMessage;
        SecondMessage = secondMessage;
        StartingSequentialNumber = startingSequentialNumber;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedAt = null;
        UpdatedBy = null;
    }

    public BilletConfiguration(CreateBilletConfigurationCommand command, Guid createdBy)
    {
        Id = Guid.NewGuid();
        CompanyCode = command.CompanyCode;
        CompanyName = command.CompanyName;
        WalletNumber = command.WalletNumber;
        AgencyNumber = command.AgencyNumber;
        AccountNumber = command.AccountNumber;
        AccountDigit = command.AccountDigit;
        BankCode = command.BankCode;
        HasFine = command.HasFine;
        FinePercentage = command.FinePercentage;
        DailyDiscount = command.DailyDiscount;
        DailyInterest = command.DailyInterest;
        DiscountLimitDate = command.DiscountLimitDate;
        DiscountAmount = command.DiscountAmount;
        RebateAmount = command.RebateAmount;
        FirstMessage = command.FirstMessage;
        SecondMessage = command.SecondMessage;
        StartingSequentialNumber = command.StartingSequentialNumber;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedAt = null;
        UpdatedBy = null;
    }

    public void Update(UpdateBilletConfigurationCommand command, Guid userId)
    {
        if (command.CompanyCode is not null) CompanyCode = command.CompanyCode;
        if (command.CompanyName is not null) CompanyName = command.CompanyName;
        if (command.WalletNumber is not null) WalletNumber = command.WalletNumber;
        if (command.AgencyNumber is not null) AgencyNumber = command.AgencyNumber;
        if (command.AccountNumber is not null) AccountNumber = command.AccountNumber;
        if (command.AccountDigit is not null) AccountDigit = command.AccountDigit;
        if (command.BankCode is not null) BankCode = command.BankCode;
        if (command.HasFine.HasValue) HasFine = command.HasFine.Value;
        if (command.FinePercentage.HasValue) FinePercentage = command.FinePercentage.Value;
        if (command.DailyDiscount.HasValue) DailyDiscount = command.DailyDiscount.Value;
        if (command.DailyInterest.HasValue) DailyInterest = command.DailyInterest.Value;
        if (command.DiscountLimitDate.HasValue) DiscountLimitDate = command.DiscountLimitDate.Value;
        if (command.DiscountAmount.HasValue) DiscountAmount = command.DiscountAmount.Value;
        if (command.RebateAmount.HasValue) RebateAmount = command.RebateAmount.Value;
        if (command.FirstMessage is not null) FirstMessage = command.FirstMessage;
        if (command.SecondMessage is not null) SecondMessage = command.SecondMessage;
        if (command.StartingSequentialNumber.HasValue) StartingSequentialNumber = command.StartingSequentialNumber.Value;

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
}