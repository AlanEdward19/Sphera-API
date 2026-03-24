namespace Sphera.API.Billing.Billets.DTOs;

public class BilletDTO
{
    public Guid ClientId { get; set; }
    public Guid ConfigurationId { get; set; }
    public Guid InstallmentId { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }

    public static BilletDTO FromEntity(Billet entity)
    {
        return new BilletDTO
        {
            Id = entity.Id,

            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,

            ClientId = entity.ClientId,
            ConfigurationId = entity.ConfigurationId,
            InstallmentId = entity.InstallmentId,
        };
    }
}