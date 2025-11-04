using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Auditory;

public sealed class AuditEntry
{
    [Key]
    public long Id { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public Guid ActorId { get; private set; }
    public string Action { get; private set; } // Create/Update/Delete
    public string EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public string? Path { get; private set; } // arquivo / blob path
    public string? Folder { get; private set; }
    public Guid CorrelationId { get; private set; }
    public string RequestIp { get; private set; }

    private AuditEntry() { }

    public AuditEntry(Guid actorId, string action, string entityType, Guid? entityId, Guid correlationId, string requestIp, string? path = null, string? folder = null)
    {
        OccurredAt = DateTime.UtcNow;
        ActorId = actorId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        CorrelationId = correlationId;
        RequestIp = requestIp ?? "unknown";
        Path = path;
        Folder = folder;
    }
}
