namespace Sphera.API.Auditory.GetAuditories;

public class GetAuditoriesQuery
{
    public long? Id { get; set; }
    public DateTime? OccurredAtStart { get; set; }
    public DateTime? OccurredAtEnd { get; set; }
    public Guid? ActorId { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public Guid? EntityId { get; set; }
}