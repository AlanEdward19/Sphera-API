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
    public DateTime IssueDate { get; private set; }

    [Required]
    public DateTime DueDate { get; private set; }

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
    public virtual ICollection<InvoiceInstallment> Installments { get; private set; } = new List<InvoiceInstallment>();

    private Invoice() { }

    public Invoice(Guid clientId, DateTime periodStart, DateTime periodEnd, Guid createdBy)
    {
        if (clientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");

        Id = Guid.NewGuid();
        ClientId = clientId;
        IssueDate = periodStart.Date;
        DueDate = periodEnd.Date;
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
    
    private void GenerateInstallments(int parcels, DateTime firstDueDate)
    {
        if (Status != EInvoiceStatus.Closed)
            throw new DomainException("A fatura deve estar fechada antes de gerar parcelas.");

        if (parcels <= 0)
            throw new DomainException("Número de parcelas inválido.");

        Installments.Clear();

        decimal amountPerParcel = Math.Round(TotalAmount / parcels, 2);
        DateTime dueDate = firstDueDate;

        for (int i = 1; i <= parcels; i++)
        {
            var installment = new InvoiceInstallment(Id, i, amountPerParcel, dueDate);
            Installments.Add(installment);

            dueDate = dueDate.AddMonths(1);
        }
    }
}