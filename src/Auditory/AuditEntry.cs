using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Users;

namespace Sphera.API.Auditory;

public class AuditEntry
{
    [Key]
    public long Id { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public Guid ActorId { get; private set; }
    public string Action { get; private set; } // Create/Update/Delete
    public string EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public string RequestIp { get; private set; }
    
    [ForeignKey(nameof(ActorId))]
    public virtual User Actor { get; private set; }

    private AuditEntry() { }

    public AuditEntry(Guid actorId, string action, string entityType, Guid? entityId, string requestIp)
    {
        OccurredAt = DateTime.UtcNow;
        ActorId = actorId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        RequestIp = requestIp;
    }
}
