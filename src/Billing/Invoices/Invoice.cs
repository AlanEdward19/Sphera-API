using Sphera.API.Billing.Invoices.Enums;
using Sphera.API.Clients;
using Sphera.API.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphera.API.Billing.Invoices;

public class Invoice
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid ClientId { get; private set; }

    [Required]
    public DateTime PeriodStart { get; private set; }

    [Required]
    public DateTime PeriodEnd { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; private set; }

    [Required]
    public EInvoiceStatus Status { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    [Required]
    public Guid CreatedBy { get; private set; }

    public DateTime? ClosedAt { get; private set; }
    public Guid? ClosedBy { get; private set; }

    public virtual Client Client { get; private set; }
    public virtual ICollection<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();

    private Invoice() { }

    public Invoice(Guid clientId, DateTime periodStart, DateTime periodEnd, Guid createdBy)
    {
        if (clientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");

        Id = Guid.NewGuid();
        ClientId = clientId;
        PeriodStart = periodStart.Date;
        PeriodEnd = periodEnd.Date;
        Status = EInvoiceStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void AddItem(Guid serviceId, string description, decimal quantity, decimal unitPrice)
    {
        var item = new InvoiceItem(Id, serviceId, description, quantity, unitPrice, 0m, false);
        Items.Add(item);
        RecalculateTotal();
    }

    public void AddAdditionalValue(string description, decimal amount)
    {
        var item = new InvoiceItem(Id, serviceId: Guid.Empty, description, 1, amount, 0m, true);
        Items.Add(item);
        RecalculateTotal();
    }

    public void Close(Guid actor)
    {
        if (!Items.Any()) throw new DomainException("Não é possível fechar fatura sem itens.");
        Status = EInvoiceStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = actor;
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.TotalAmount);
    }
}