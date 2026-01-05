using Sphera.API.Shared.Enums;

namespace Sphera.API.Reports.GenerateClientsReport;

public class ClientsReportDTO
{
    public string TradeName { get; private set; }
    public string LegalName { get; private set; }
    public string Cnpj { get; private set; }
    public Guid PartnerId { get; private set; }
    public string PartnerName { get; private set; }
    public DateTime? EcacExpirationDate { get; private set; }
    public EExpirationStatus? Status { get; private set; }

    public ClientsReportDTO(string tradeName, string legalName, string cnpj, Guid partnerId, string partnerName, DateTime? ecacExpirationDate, EExpirationStatus? status)
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