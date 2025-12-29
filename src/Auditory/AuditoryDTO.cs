namespace Sphera.API.Auditory;

public class AuditoryDTO
{
    public long Id { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public Guid ActorId { get; private set; }
    public string Action { get; private set; }
    public string EntityType { get; private set; }
    public string EntityName { get; private set; }
    public Guid? EntityId { get; private set; }
    public string RequestIp { get; private set; }
    public string ActorEmail { get; private set; }
    public string ActorName { get; private set; }

    public AuditoryDTO(long id, DateTime occurredAt, Guid actorId, string action, string entityType, string entityName,
        Guid? entityId,
        string requestIp, string actorEmail, string actorName)
    {
        Id = id;
        OccurredAt = occurredAt;
        ActorId = actorId;
        Action = action;
        EntityType = entityType;
        EntityName = entityName;
        EntityId = entityId;
        RequestIp = requestIp;
        ActorEmail = actorEmail;
        ActorName = actorName;
    }
}