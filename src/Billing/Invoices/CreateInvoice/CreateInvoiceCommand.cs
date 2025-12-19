using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Billing.Invoices.CreateInvoice;

public class CreateInvoiceCommand
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    public List<CreateInvoiceItem> Items { get; set; } = new();

    public List<CreateInvoiceInstallment> Installments { get; set; } = new();
}

public class CreateInvoiceItem
{
    public Guid? ServiceId { get; set; }
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AdditionalAmount { get; set; } = 0m;

    public bool IsAdditional { get; set; } = false;
}

public class CreateInvoiceInstallment
{
    [Range(1, int.MaxValue)]
    public int Number { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
}
