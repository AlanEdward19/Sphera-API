using System.ComponentModel.DataAnnotations;
using NuGet.Packaging;
using Sphera.API.Billing.Billets;

namespace Sphera.API.Billing.Remittances;

public class Remittance
{
    [Key]
    public Guid Id { get; private set; }
    
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
    
    public virtual ICollection<Billet> Billets { get; set; } = new List<Billet>();

    public Remittance() { }

    public Remittance(string fileName, Guid createdBy)
    {
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
        if (Billets.All(b => b.Id != billet.Id))
        {
            Billets.Add(billet);
        }
    }
    
    public void AddBillets(IEnumerable<Billet> billets)
    {
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
    }
}