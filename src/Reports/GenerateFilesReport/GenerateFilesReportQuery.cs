using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateFilesReport;

public class GenerateFilesReportQuery
{
    public Guid? PartnerId { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? ServiceId { get; set; }
    public EExpirationStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}