using Sphera.API.Services;
using Sphera.API.Shared;
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

    /// <summary>
    /// Indica se o item foi criado com preço manual permitido (AllowManual),
    /// normalmente quando não havia preço configurado no fechamento e foi aplicado 0.
    /// </summary>
    [Required]
    public bool IsManualPriced { get; private set; }

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
        bool isAdditional,
        bool isManualPriced = false)
    {
        Id = Guid.NewGuid();
        InvoiceId = invoiceId;
        ServiceId = serviceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        AdditionalAmount = additionalAmount;
        IsAdditional = isAdditional;
        IsManualPriced = isManualPriced;
        TotalAmount = (quantity * unitPrice) + additionalAmount;
    }

    public void UpdateManualValues(decimal quantity, decimal unitPrice)
    {
        if (!IsManualPriced)
            throw new DomainException("Edição permitida apenas para itens com preço manual.");

        if (IsAdditional)
            throw new DomainException("Itens adicionais não podem ser editados por este endpoint.");

        if (quantity <= 0)
            throw new DomainException("Quantidade deve ser maior que zero.");

        if (unitPrice < 0)
            throw new DomainException("Preço unitário não pode ser negativo.");

        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalAmount = (quantity * unitPrice) + AdditionalAmount;
    }
}