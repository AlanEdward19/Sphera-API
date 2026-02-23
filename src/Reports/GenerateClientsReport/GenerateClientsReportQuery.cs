using Sphera.API.Clients.Enums;
using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateClientsReport;

/// <summary>
/// Represents a query used to generate a report of clients based on specified criteria.
/// </summary>
/// <remarks>
/// This query encapsulates parameters such as a specific partner identifier, client status,
/// and date filters to customize the clients report generation. It is used as input for
/// report generation handlers to retrieve and format the relevant client data.
/// </remarks>
public class GenerateClientsReportQuery
{
    /// <summary>
    /// Gets or sets the identifier of the partner associated with the client.
    /// </summary>
    /// <remarks>
    /// This property is used to filter clients by the associated partner during the generation of client reports.
    /// The value is optional and can be null.
    /// </remarks>
    public Guid? PartnerId { get; set; }

    /// <summary>
    /// Gets or sets the expiration status associated with the report query.
    /// </summary>
    /// <remarks>
    /// This property is used to filter clients based on their subscription expiration status
    /// when generating the client report. The value is optional and can be null.
    /// </remarks>
    public EExpirationStatus? Status { get; set; }
    
    /// <summary>
    /// Gets or sets the payment status used for filtering clients in the report.
    /// </summary> <remarks>
    /// This property is used to filter clients based on their payment status when generating the client report. The value is optional and can be null.
    /// </remarks>
    public EPaymentStatus? PaymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the starting date for filtering client records.
    /// </summary>
    /// <remarks>
    /// This property is used to include only the clients whose Ecac expiration date
    /// is on or after this date in the generated report. The value is optional and can be null.
    /// </remarks>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Gets or sets the end date used for filtering clients in the report.
    /// </summary>
    /// <remarks>
    /// When generating a client report, this property determines the latest date
    /// for filtering clients based on their expiration date. If set to a value,
    /// only clients with an expiration date on or before this date are included in the report.
    /// The value is optional and can be null.
    /// </remarks>
    public DateTime? ToDate { get; set; }
}