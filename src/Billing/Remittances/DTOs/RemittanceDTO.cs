using Sphera.API.Billing.Billets;

namespace Sphera.API.Billing.Remittances.DTOs;

public class RemittanceDTO
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public bool IsSubmitted { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public IReadOnlyCollection<Guid> BilletIds { get; set; } = Array.Empty<Guid>();

    public static RemittanceDTO FromEntity(Remittance entity)
    {
        return new RemittanceDTO
        {
            Id = entity.Id,
            FileName = entity.FileName,
            IsSubmitted = entity.IsSubmitted,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy,
            BilletIds = entity.Billets.Select(b => b.Id).ToArray()
        };
    }
}
