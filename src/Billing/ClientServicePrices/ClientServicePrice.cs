using Sphera.API.Clients;
using Sphera.API.Services;
using Sphera.API.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphera.API.Billing.ClientServicePrices;

public class ClientServicePrice
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid ClientId { get; private set; }

    [Required]
    public Guid ServiceId { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; private set; }

    [Required]
    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    [Required]
    public bool IsActive { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    [Required]
    public Guid CreatedBy { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    [ForeignKey(nameof(ClientId))]
    public virtual Client Client { get; private set; }

    [ForeignKey(nameof(ServiceId))]
    public virtual Service Service { get; private set; }

    private ClientServicePrice() { }

    public ClientServicePrice(Guid clientId, Guid serviceId, decimal unitPrice, DateTime startDate, Guid createdBy)
    {
        if (clientId == Guid.Empty) throw new DomainException("ClientId obrigatório.");
        if (serviceId == Guid.Empty) throw new DomainException("ServiceId obrigatório.");
        if (unitPrice <= 0) throw new DomainException("Preço deve ser maior que zero.");

        Id = Guid.NewGuid();
        ClientId = clientId;
        ServiceId = serviceId;
        UnitPrice = unitPrice;
        StartDate = startDate.Date;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdatePrice(decimal unitPrice, DateTime startDate, Guid actor)
    {
        if (unitPrice <= 0) throw new DomainException("Preço deve ser maior que zero.");

        UnitPrice = unitPrice;
        StartDate = startDate.Date;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void Deactivate(Guid actor)
    {
        IsActive = false;
        EndDate ??= DateTime.UtcNow.Date;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }
}
