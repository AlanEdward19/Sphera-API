namespace Sphera.API.Services.GetServices;

/// <summary>
/// Represents the parameters used to query for services based on code, name, or active status.
/// </summary>
public class GetServicesQuery
{
    /// <summary>
    /// Gets or sets the code associated with the current entity.
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the entity is active.
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Specifies the current page number for paginated document queries.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Specifies the maximum number of documents to be retrieved per page in a paginated query.
    /// </summary>
    public int PageSize { get; set; } = 10;
}