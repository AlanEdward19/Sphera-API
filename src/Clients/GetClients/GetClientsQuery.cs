using Sphera.API.Shared.Enums;

namespace Sphera.API.Clients.GetClients;

/// <summary>
/// Represents a query for retrieving client information. This query allows
/// filtering based on partner ID, client status, CNPJ, and search criteria.
/// </summary>
public class GetClientsQuery
{
    /// <summary>
    /// Gets or sets the optional identifier for the partner associated with the query.
    /// This property may be used to filter clients based on a specific partner relationship.
    /// </summary>
    public Guid? PartnerId { get; set; }

    /// <summary>
    /// Gets or sets the status indicating the optional active or inactive state of the client query.
    /// This property may be used to filter clients based on their current status.
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// Gets or sets the optional CNPJ (Cadastro Nacional da Pessoa Jurídica) used to filter clients.
    /// This property allows specifying a unique identifier for a legal entity in Brazil.
    /// </summary>
    public string? Cnpj { get; set; }
    
    /// <summary>
    /// Current status of the clients, indicating whether it is within the deadline, about to expire, or expired.
    /// </summary>
    public EExpirationStatus? ExpirationStatus { get; set; }

    /// <summary>
    /// The start date for filtering clients based on their due date.
    /// </summary>
    public DateTime? DueDateFrom { get; set; }

    /// <summary>
    /// Specifies the upper bound for the due date range to filter clients, allowing retrieval of documents
    /// with due dates up to the specified value.
    /// </summary>
    public DateTime? DueDateTo { get; set; }

    /// <summary>
    /// Gets or sets the search term used to filter clients based on their attributes.
    /// This property can accept partial or full values to match client records.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Specifies the current page number for paginated document queries.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Specifies the maximum number of documents to be retrieved per page in a paginated query.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether partner-related data should be included in the results.
    /// </summary>
    public bool? IncludePartner { get; set; } = false;
}