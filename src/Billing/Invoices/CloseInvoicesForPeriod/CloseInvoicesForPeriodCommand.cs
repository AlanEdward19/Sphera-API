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
    /// Se informado, fecha só de um cliente. Se null, todos.
    /// </summary>
    public Guid? ClientId { get; set; }

    [Required]
    public EMissingPriceBehavior MissingPriceBehavior { get; set; }
}