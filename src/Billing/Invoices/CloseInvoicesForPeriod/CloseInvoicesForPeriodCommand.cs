using System.ComponentModel.DataAnnotations;
using Sphera.API.Billing.Invoices.Enums;

namespace Sphera.API.Billing.Invoices.CloseInvoicesForPeriod;

public class CloseInvoicesForPeriodCommand
{
    [Required]
    public DateTime PeriodStart { get; set; }

    [Required]
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Cliente obrigatório para fechamento.
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public EMissingPriceBehavior MissingPriceBehavior { get; set; }

    /// <summary>
    /// Cenário 1: se enviado, cria uma parcela única com este valor.
    /// </summary>
    [Range(0.01, double.MaxValue)]
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Cenário 1: se enviado, define o vencimento da parcela única/da fatura.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Cenário 2: lista de parcelas para a fatura.
    /// </summary>
    public List<CloseInvoiceInstallment> Installments { get; set; } = new();
}

public class CloseInvoiceInstallment
{
    [Range(1, int.MaxValue)]
    public int Number { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
}