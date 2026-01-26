using Sphera.API.Contacts;
using Sphera.API.Shared.DTOs;
using System.Collections.ObjectModel;
using Sphera.API.Clients.Enums;
using Sphera.API.Partners.DTOs;
using Sphera.API.Shared.Enums;

namespace Sphera.API.Clients.DTOs;

public class ClientWithPartnerDTO : ClientDTO
{
    public PartnerDTO Partner { get; private set; }
    public ClientWithPartnerDTO(Guid id, string tradeName, string legalName, string cnpj, string? stateRegistration, string? municipalRegistration, 
        AddressDTO address, short? billingDueDay, DateTime? contractDate, EExpirationStatus? expirationStatus, bool status, DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy, 
        ReadOnlyCollection<ContactDTO> contacts, int documentCount, string? notes, DateTime? ecacExpirationDate, EPaymentStatus? paymentStatus, PartnerDTO partner) : base(id, tradeName, legalName, cnpj, stateRegistration, municipalRegistration, address, 
            billingDueDay, contractDate, expirationStatus, status, createdAt, createdBy, updatedAt, updatedBy, contacts, documentCount, notes, ecacExpirationDate, paymentStatus)
    {
        Partner = partner;
    }
}
