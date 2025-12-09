using Sphera.API.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphera.API.Billing.Invoices;

public class InvoiceItem
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid InvoiceId { get; private set; }

    public Guid? ServiceId { get; private set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantity { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AdditionalAmount { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; private set; }

    [Required]
    public bool IsAdditional { get; private set; }

    public virtual Invoice Invoice { get; private set; }
    public virtual Service? Service { get; private set; }

    private InvoiceItem() { }

    public InvoiceItem(
        Guid invoiceId,
        Guid? serviceId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal additionalAmount,
        bool isAdditional)
    {
        Id = Guid.NewGuid();
        InvoiceId = invoiceId;
        ServiceId = serviceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        AdditionalAmount = additionalAmount;
        IsAdditional = isAdditional;
        TotalAmount = (quantity * unitPrice) + additionalAmount;
    }
}