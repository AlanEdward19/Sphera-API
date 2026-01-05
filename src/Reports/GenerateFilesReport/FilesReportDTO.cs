using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateFilesReport;

public class FilesReportDTO
{
    public string FileName { get; private set; }
    public Guid PartnerId { get; private set; }
    public string PartnerName { get; private set; }
    public Guid ClientId { get; private set; }
    public string ClientName { get; private set; }
    public Guid ServiceId { get; private set; }
    public string ServiceName { get; private set; }
    public Guid ResponsibleId { get; private set; }
    public string ResponsibleName { get; private set; }
    public DateTime DueDate { get; private set; }
    public EExpirationStatus Status { get; private set; }

    public FilesReportDTO(string fileName, Guid partnerId, string partnerName, Guid clientId, string clientName,
        Guid serviceId, string serviceName, Guid responsibleId, string responsibleName, DateTime dueDate,
        EExpirationStatus status)
    {
        FileName = fileName;
        PartnerId = partnerId;
        PartnerName = partnerName;
        ClientId = clientId;
        ClientName = clientName;
        ServiceId = serviceId;
        ServiceName = serviceName;
        ResponsibleId = responsibleId;
        ResponsibleName = responsibleName;
        DueDate = dueDate;
        Status = status;
    }
}