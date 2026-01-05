using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateClientsReport;

public class GenerateClientsReportQuery
{
    public Guid? PartnerId { get; set; }
    public EExpirationStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}