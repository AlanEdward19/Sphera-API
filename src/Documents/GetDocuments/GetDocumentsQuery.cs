using Sphera.API.Shared.Enums;

namespace Sphera.API.Documents.GetDocuments;

/// <summary>
/// Represents a query object for retrieving documents with filtering and pagination options.
/// </summary>
public class GetDocumentsQuery
{
    /// <summary>
    /// Identifier of the partner associated with the document query, used to filter documents by a specific partner.
    /// </summary>
    public Guid? PartnerId { get; set; }

    /// <summary>
    /// Identifier of the client associated with the document query, used to filter documents by a specific client.
    /// </summary>
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Identifier of the service associated with the document query, used to filter documents by a specific service.
    /// </summary>
    public Guid? ServiceId { get; set; }

    /// <summary>
    /// Current status of the document, indicating whether it is within the deadline, about to expire, or expired.
    /// </summary>
    public EExpirationStatus? Status { get; set; }

    /// <summary>
    /// The start date for filtering documents based on their due date.
    /// </summary>
    public DateTime? DueDateFrom { get; set; }

    /// <summary>
    /// Specifies the upper bound for the due date range to filter documents, allowing retrieval of documents
    /// with due dates up to the specified value.
    /// </summary>
    public DateTime? DueDateTo { get; set; }

    /// <summary>
    /// Text input utilized for searching and filtering documents based on their content or associated metadata.
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
}