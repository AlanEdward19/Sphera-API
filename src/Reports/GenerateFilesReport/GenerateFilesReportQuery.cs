using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateFilesReport;

/// <summary>
/// Represents a query object for generating a files report.
/// This query contains optional parameters to filter and retrieve specific data
/// related to file reports based on provided criteria.
/// </summary>
public class GenerateFilesReportQuery
{
    /// <summary>
    /// Gets or sets the identifier of the partner associated with the report query.
    /// </summary>
    /// <remarks>
    /// This property is used to filter the documents in the report based on their associated partner.
    /// If a value is provided, only documents related to the specified partner will be included in the report.
    /// </remarks>
    public Guid? PartnerId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the client associated with the report query.
    /// </summary>
    /// <remarks>
    /// This property is used to filter the documents in the report based on their associated client.
    /// If a value is provided, only documents related to the specified client will be included in the report.
    /// </remarks>
    public Guid? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the service associated with the report query.
    /// </summary>
    /// <remarks>
    /// This property is used to filter the documents in the report based on their associated service.
    /// If a value is provided, only documents related to the specified service will be included in the report.
    /// </remarks>
    public Guid? ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the expiration status used to filter the documents in the report query.
    /// </summary>
    /// <remarks>
    /// This property determines which documents are included in the report based on their expiration status.
    /// Possible values correspond to the states defined in the <c>EExpirationStatus</c> enumeration.
    /// </remarks>
    public EExpirationStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the start date for filtering documents in the report query.
    /// </summary>
    /// <remarks>
    /// This property is used to limit the results to documents whose due dates are on or after the specified start date.
    /// If no value is provided, documents will not be filtered based on a start date.
    /// </remarks>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Gets or sets the filter for the maximum due date of documents to be included in the report.
    /// </summary>
    /// <remarks>
    /// This property is used to retrieve documents with a due date less than or equal to the specified value.
    /// If left null, no upper limit will be applied to the due date filter in the query.
    /// </remarks>
    public DateTime? ToDate { get; set; }
}