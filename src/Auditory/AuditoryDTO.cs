namespace Sphera.API.Auditory;

public class AuditoryDTO
{
    public long Id { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public Guid ActorId { get; private set; }
    public string Action { get; private set; }
    public string EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public string RequestIp { get; private set; }
    public string ActorEmail { get; private set; }
    
    public AuditoryDTO(long id, DateTime occurredAt, Guid actorId, string action, string entityType, Guid? entityId, string requestIp,  string actorEmail)
    {
        Id = id;
        OccurredAt = occurredAt;
        ActorId = actorId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        RequestIp = requestIp;
        ActorEmail = actorEmail;
    }
}