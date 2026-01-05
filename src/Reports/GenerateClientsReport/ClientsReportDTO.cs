using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateClientsReport;

/// <summary>
/// Represents a data transfer object for clients report information.
/// This DTO is used to encapsulate the details of a client for reporting purposes.
/// </summary>
/// <remarks>
/// The <see cref="ClientsReportDTO"/> class includes information such as the client's trade name,
/// legal name, CNPJ (Brazilian corporate taxpayer registry identification), partner details,
/// eCAC (Virtual Taxpayer Service Center) expiration date, and the expiration status.
/// Instances of this class are immutable and are populated using the constructor.
/// </remarks>
public class ClientsReportDTO
{
    /// <summary>
    /// Gets the trade name of the client.
    /// </summary>
    /// <remarks>
    /// The trade name is the commercial or business name under which the
    /// client operates. It is commonly used in business communications
    /// and may differ from the client's legal name.
    /// </remarks>
    public string TradeName { get; private set; }

    /// <summary>
    /// Gets the legal name of the client.
    /// </summary>
    /// <remarks>
    /// The legal name is the officially registered name of the client as recognized
    /// by government or regulatory entities. It is typically used in formal documents
    /// and legal proceedings to identify the client in an official capacity.
    /// </remarks>
    public string LegalName { get; private set; }

    /// <summary>
    /// Gets the CNPJ (Cadastro Nacional da Pessoa Jurídica) of the client.
    /// </summary>
    /// <remarks>
    /// The CNPJ is a unique identifier issued by the Brazilian Federal Revenue service
    /// for corporate entities operating in Brazil. It is a mandatory registration
    /// for all businesses in the country and is used for tax purposes.
    /// </remarks>
    public string Cnpj { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the partner associated with the client.
    /// </summary>
    /// <remarks>
    /// The partner identifier is a <see cref="Guid"/> that uniquely distinguishes
    /// a business partner related to the client. This property serves as a reference
    /// for linking client records to their corresponding partner in the database
    /// or other systems.
    /// </remarks>
    public Guid PartnerId { get; private set; }

    /// <summary>
    /// Gets the name of the partner associated with the client.
    /// </summary>
    /// <remarks>
    /// The partner name represents the individual or entity that has a formal partnership
    /// with the client. This property is used to identify the partner in reports and
    /// other client documentation.
    /// </remarks>
    public string PartnerName { get; private set; }

    /// <summary>
    /// Gets the expiration date of the eCAC (Virtual Taxpayer Service Center) for the client.
    /// </summary>
    /// <remarks>
    /// The eCAC expiration date indicates the validity period for the client's authorization or certificates
    /// used to access services in the Virtual Taxpayer Service Center. This date is essential for determining
    /// whether the client can continue to access services without interruption.
    /// </remarks>
    public DateTime? EcacExpirationDate { get; private set; }

    /// <summary>
    /// Gets the expiration status of the client's eCAC (Virtual Taxpayer Service Center) credentials.
    /// </summary>
    /// <remarks>
    /// The status indicates whether the client's eCAC credentials are within the validity period,
    /// approaching expiration, or have already expired. This information is critical for compliance
    /// and renewal tracking in tax-related processes.
    /// </remarks>
    public EExpirationStatus? Status { get; private set; }

    /// <summary>
    /// A Data Transfer Object (DTO) that represents the report of clients.
    /// </summary>
    public ClientsReportDTO(string tradeName, string legalName, string cnpj, Guid partnerId, string partnerName,
        DateTime? ecacExpirationDate, EExpirationStatus? status)
    {
        TradeName = tradeName;
        LegalName = legalName;
        Cnpj = cnpj;
        PartnerId = partnerId;
        PartnerName = partnerName;
        EcacExpirationDate = ecacExpirationDate;
        Status = status;
    }
}