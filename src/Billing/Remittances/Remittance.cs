using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Packaging;
using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets;
using Sphera.API.Billing.Billets.Enums;

namespace Sphera.API.Billing.Remittances;

public class Remittance
{
    [Key]
    public Guid Id { get; private set; }
    
    public EBilletBank? Bank { get; private set; }
    
    public string FileName { get; private set; }
    
    public bool IsSubmitted { get; private set; }
    
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
    
    public Guid? ConfigurationId { get; set; }
    
    [ForeignKey(nameof(ConfigurationId))]
    public virtual BilletConfiguration? Configuration { get; set; }
    
    public virtual ICollection<Billet> Billets { get; set; } = new List<Billet>();

    public Remittance() { }

    public Remittance(string fileName, Guid createdBy)
    {
        Id = Guid.NewGuid();
        FileName = fileName;
        IsSubmitted = false;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedAt = null;
        UpdatedBy = null;
    }
    
    public void MarkAsSubmitted(Guid userId)
    {
        IsSubmitted = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
    
    public void UpdateFileName(string newFileName, Guid userId)
    {
        FileName = newFileName;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
    
    public void AddBillet(Billet billet)
    {
        if (Bank == null)
        {
            Bank = billet.Bank;
        }
        else if (billet.Bank != Bank)
        {
            throw new InvalidOperationException("Cannot add billet from a different bank to this remittance.");
        }

        if (ConfigurationId == null)
        {
            ConfigurationId = billet.ConfigurationId;
        }
        else if (billet.ConfigurationId != ConfigurationId)
        {
            throw new InvalidOperationException("Cannot add billet with a different configuration to this remittance.");
        }
        
        if (Billets.All(b => b.Id != billet.Id))
        {
            Billets.Add(billet);
        }
    }
    
    public void AddBillets(IEnumerable<Billet> billets)
    {
        var billetsBanks = billets.Select(b => b.Bank).Distinct().ToList();
        var billetsConfigurations = billets.Select(b => b.ConfigurationId).Distinct().ToList();
        if (Bank == null)
        {
            if (billetsBanks.Count > 1)
            {
                throw new InvalidOperationException("Cannot add billets from different banks to this remittance.");
            }
            Bank = billetsBanks.First();
        }
        else if (billetsBanks.Any(b => b != Bank))
        {
            throw new InvalidOperationException("Cannot add billets from a different bank to this remittance.");
        }
        if (ConfigurationId == null)
        {
            if (billetsConfigurations.Count > 1)
            {
                throw new InvalidOperationException("Cannot add billets with different configurations to this remittance.");
            }
            ConfigurationId = billetsConfigurations.First();
        }
        else if (billetsConfigurations.Any(c => c != ConfigurationId))
        {
            throw new InvalidOperationException("Cannot add billets with a different configuration to this remittance.");
        }
        
        Billets.AddRange(billets);
        Billets = Billets
            .DistinctBy(b => b.Id)
            .ToList();
    }
    
    public void RemoveBillet(Billet billet)
    {
        Billets = Billets
            .Where(b => b.Id == billet.Id)
            .ToList();
        
        if (!Billets.Any())
        {
            Bank = null;
            ConfigurationId = null;
        }
    }
}