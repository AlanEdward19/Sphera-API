namespace Sphera.API.Partners.GetPartners;

/// <summary>
/// Represents a query object for retrieving partners with filter and pagination options.
/// </summary>
public class GetPartnersQuery
{
    /// <summary>
    /// Represents the search criteria for retrieving partners based on specific query strings.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Indicates the status for filtering partners in the query.
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// Represents the CNPJ value for filtering partners in the query.
    /// </summary>
    public string? Cnpj { get; set; }

    /// <summary>
    /// Specifies the current page number for paginated document queries.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Specifies the maximum number of documents to be retrieved per page in a paginated query.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether client information should be included in the results.
    /// </summary>
    public bool? IncludeClients { get; set; } = false;
}