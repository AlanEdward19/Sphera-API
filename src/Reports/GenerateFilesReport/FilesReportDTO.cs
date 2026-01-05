using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateFilesReport;

public class FilesReportDTO
{
    /// <summary>
    /// Gets the name of the file associated with the report.
    /// </summary>
    /// <remarks>
    /// This property represents the specific filename used in the generated files report.
    /// It is assigned during the creation of the <see cref="FilesReportDTO"/> object and cannot be modified later.
    /// </remarks>
    public string FileName { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the partner associated with the report.
    /// </summary>
    /// <remarks>
    /// This property represents the partner's unique identifier that is provided when creating the <see cref="FilesReportDTO"/> object.
    /// It helps to associate the report with a specific partner entity for tracking and identification purposes.
    /// </remarks>
    public Guid PartnerId { get; private set; }

    /// <summary>
    /// Gets the name of the partner associated with the report.
    /// </summary>
    /// <remarks>
    /// This property represents the name of the partner tied to the generated report.
    /// It is initialized during the creation of the <see cref="FilesReportDTO"/> object and cannot be modified afterward.
    /// </remarks>
    public string PartnerName { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the client associated with the report.
    /// </summary>
    /// <remarks>
    /// This property represents the globally unique identifier (GUID) of the client linked to the generated files report.
    /// It is assigned during the initialization of the <see cref="FilesReportDTO"/> object and cannot be modified afterwards.
    /// </remarks>
    public Guid ClientId { get; private set; }

    /// <summary>
    /// Gets the name of the client associated with the report.
    /// </summary>
    /// <remarks>
    /// This property holds the name of the client to whom the generated files report is related.
    /// The value is provided during the creation of the <see cref="FilesReportDTO"/> object and cannot be modified afterward.
    /// </remarks>
    public string ClientName { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the service associated with the report.
    /// </summary>
    /// <remarks>
    /// This property represents the unique <see cref="Guid"/> used to identify the specific service
    /// referenced in the generated files report. It is set during the instantiation of the
    /// <see cref="FilesReportDTO"/> object and remains immutable throughout its lifecycle.
    /// </remarks>
    public Guid ServiceId { get; private set; }

    /// <summary>
    /// Gets the name of the service associated with the report.
    /// </summary>
    /// <remarks>
    /// This property denotes the specific service name linked to the report generation process.
    /// It is initialized during the creation of the <see cref="FilesReportDTO"/> object and cannot be changed thereafter.
    /// </remarks>
    public string ServiceName { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the responsible user associated with the report.
    /// </summary>
    /// <remarks>
    /// This property represents the unique <see cref="Guid"/> of the user assigned as responsible
    /// for the task or report. It is initialized during the creation of the <see cref="FilesReportDTO"/>
    /// instance and cannot be modified afterwards.
    /// </remarks>
    public Guid ResponsibleId { get; private set; }

    /// <summary>
    /// Gets the name of the individual responsible for the associated report.
    /// </summary>
    /// <remarks>
    /// This property represents the name of the person assigned to oversee the delivery
    /// or management of the report. It is set during the initialization of the
    /// <see cref="FilesReportDTO"/> object and cannot be updated afterward.
    /// </remarks>
    public string ResponsibleName { get; private set; }

    /// <summary>
    /// Gets the due date associated with the report.
    /// </summary>
    /// <remarks>
    /// This property indicates the deadline or expected completion date for the report.
    /// It is assigned during the creation of the <see cref="FilesReportDTO"/> object and cannot be modified afterward.
    /// </remarks>
    public DateTime DueDate { get; private set; }

    /// <summary>
    /// Gets the expiration status of the associated file report.
    /// </summary>
    /// <remarks>
    /// This property indicates the current status of the file report's expiration, as defined by the <see cref="EExpirationStatus"/> enumeration.
    /// The value is determined at the time of object creation and provides an overview of whether the report is within the deadline,
    /// nearing expiration, or already expired.
    /// </remarks>
    public EExpirationStatus Status { get; private set; }

    /// <summary>
    /// Represents a data transfer object designed to encapsulate information
    /// about a files report for reporting and data handling purposes.
    /// </summary>
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