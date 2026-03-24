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

    [MaxLength(200)]
    public string? Name { get; private set; }

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

    public bool? IsSentToReceivables { get; private set; } = false;
    public virtual Client Client { get; private set; }
    public virtual ICollection<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();
    public virtual ICollection<InvoiceInstallment> Installments { get; private set; } = new List<InvoiceInstallment>();

    private Invoice() { }

    public Invoice(Guid clientId, DateTime issueDate, DateTime dueDate, Guid createdBy)
    {
        if (clientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");

        Id = Guid.NewGuid();
        ClientId = clientId;
        IssueDate = issueDate.Date;
        DueDate = dueDate.Date;
        Status = EInvoiceStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Name = null;
            return;
        }

        Name = name.Trim();
    }

    public void AddItem(Guid serviceId, string description, decimal quantity, decimal unitPrice, bool isManualPriced = false)
    {
        var item = new InvoiceItem(Id, serviceId, description, quantity, unitPrice, 0m, false, isManualPriced);
        Items.Add(item);
        RecalculateTotal();
    }

    public void AddAdditionalValue(string description, decimal amount)
    {
        var item = new InvoiceItem(Id, serviceId: Items.First().ServiceId, description, 1, amount, 0m, true);
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

    public void RecalculateTotalAmount()
    {
        RecalculateTotal();
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

    public void SetInstallments(IEnumerable<(int Number, decimal Amount, DateTime DueDate)> installments)
    {
        if (installments is null) throw new DomainException("Parcelas não podem ser nulas.");

        var list = installments.ToList();
        if (!list.Any()) throw new DomainException("É necessário informar ao menos uma parcela.");

        Installments.Clear();

        foreach (var inst in list)
        {
            if (inst.Number <= 0) throw new DomainException("Número da parcela inválido.");
            if (inst.Amount <= 0) throw new DomainException("Valor da parcela inválido.");

            var entity = new InvoiceInstallment(Id, inst.Number, inst.Amount, inst.DueDate.Date);
            Installments.Add(entity);
        }

        var lastDue = Installments.Max(i => i.DueDate);
        if (DueDate.Date != lastDue.Date)
            throw new DomainException("Data de vencimento da fatura deve ser igual à data da última parcela.");
    }
    
    public void ToggleSentToReceivables()
    {
        IsSentToReceivables = !IsSentToReceivables ?? true;
    }
}