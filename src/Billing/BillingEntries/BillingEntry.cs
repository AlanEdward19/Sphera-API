using Sphera.API.Clients;
using Sphera.API.Services;
using Sphera.API.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphera.API.Billing.BillingEntries;

public class BillingEntry
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid ClientId { get; private set; }

    [Required]
    public Guid ServiceId { get; private set; }

    [Required]
    public DateTime ServiceDate { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Quantity { get; private set; }

    [Required]
    public bool IsBillable { get; private set; }

    [MaxLength(500)]
    public string? Notes { get; private set; }

    public Guid? InvoiceId { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    [Required]
    public Guid CreatedBy { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; }

    public virtual Client Client { get; private set; }
    public virtual Service Service { get; private set; }

    private BillingEntry() { }

    public BillingEntry(Guid clientId, Guid serviceId, decimal quantity, DateTime serviceDate, string? notes, Guid createdBy)
    {
        if (clientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");
        if (serviceId == Guid.Empty) throw new DomainException("ServiceId obrigatório.");
        if (quantity <= 0) throw new DomainException("Quantidade deve ser maior que zero.");

        Id = Guid.NewGuid();
        ClientId = clientId;
        ServiceId = serviceId;
        Quantity = quantity;
        ServiceDate = serviceDate.Date;
        Notes = notes;
        IsBillable = true; // regra: padrão faturável
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetBillable(bool billable, Guid actor)
    {
        IsBillable = billable;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void SetNotes(string? notes, Guid actor)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void UpdateQuantity(decimal quantity, Guid actor)
    {
        if (quantity <= 0) throw new DomainException("Quantidade deve ser maior que zero.");
        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void AttachToInvoice(Guid invoiceId)
    {
        InvoiceId = invoiceId;
    }
}