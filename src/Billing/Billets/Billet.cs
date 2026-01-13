using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets.Enums;
using Sphera.API.Billing.Invoices;
using Sphera.API.Clients;
using Sphera.API.Billing.Remittances;

namespace Sphera.API.Billing.Billets;

public class Billet
{
    public Guid Id { get; private set; }
    
    public EBilletBank Bank { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who created the entity.
    /// </summary>
    [Required]
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; private set; }
    
    public Guid InstallmentId { get; set; }
    
    [ForeignKey(nameof(InstallmentId))]
    public virtual InvoiceInstallment Installment { get; set; }
    
    public Guid ConfigurationId { get; set; }
    
    [ForeignKey(nameof(ConfigurationId))]
    public BilletConfiguration Configuration { get; set; }
    
    public Guid ClientId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public virtual Client Client { get; private set; }

    public Guid? RemittanceId { get; set; }

    [ForeignKey(nameof(RemittanceId))]
    public virtual Remittance? Remittance { get; set; }

    public Billet() { }

    public Billet(EBilletBank bank, Guid createdBy, Guid installmentId, Guid configurationId, Guid clientId)
    {
        Id = Guid.NewGuid();
        Bank = bank;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedAt = null;
        UpdatedBy = null;
        
        InstallmentId = installmentId;
        ConfigurationId = configurationId;
        ClientId = clientId;
    }
}