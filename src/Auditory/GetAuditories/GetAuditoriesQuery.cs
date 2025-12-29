namespace Sphera.API.Auditory.GetAuditories;

public class GetAuditoriesQuery
{
    /// <summary>
    /// Specifies the unique identifier of the audit entry.
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Specifies the date and time when the audit entry started.
    /// </summary>
    public DateTime? OccurredAtStart { get; set; }
    
    /// <summary>
    /// Specifies the date and time when the audit entry ended.
    /// </summary>
    public DateTime? OccurredAtEnd { get; set; }
    
    /// <summary>
    /// Specifies the unique identifier of the user who initiated the audit entry.
    /// </summary>
    public Guid? ActorId { get; set; }
    
    /// <summary>
    /// Specifies the action associated with the audit entry.
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Specifies the type of entity associated with the audit entry.
    /// </summary>
    public string? EntityType { get; set; }
    
    /// <summary>
    /// Specifies the unique identifier of the entity associated with the audit entry.
    /// </summary>
    public Guid? EntityId { get; set; }
    
    /// <summary>
    /// Specifies the current page number for paginated document queries.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Specifies the maximum number of documents to be retrieved per page in a paginated query.
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Text input utilized for searching and filtering auditories based on their content or associated metadata.
    /// </summary>
    public string? Search { get; set; }
}