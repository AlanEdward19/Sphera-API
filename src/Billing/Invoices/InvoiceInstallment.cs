using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Billing.Invoices.Enums;

namespace Sphera.API.Billing.Invoices;

public class InvoiceInstallment
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid InvoiceId { get; private set; }

    [Required]
    public int Number { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; private set; }

    [Required]
    public DateTime DueDate { get; private set; }

    [Required]
    public EInstallmentStatus Status { get; private set; }

    public virtual Invoice Invoice { get; private set; }

    private InvoiceInstallment() { }

    public InvoiceInstallment(Guid invoiceId, int number, decimal amount, DateTime dueDate)
    {
        Id = Guid.NewGuid();
        InvoiceId = invoiceId;
        Number = number;
        Amount = amount;
        DueDate = dueDate;
        Status = EInstallmentStatus.Open;
    }

    public void MarkAsPaid()
    {
        Status = EInstallmentStatus.Paid;
    }

    public void Cancel()
    {
        Status = EInstallmentStatus.Cancelled;
    }
}