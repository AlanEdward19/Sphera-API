using Sphera.API.Contacts;
using Sphera.API.Shared.DTOs;
using System.Collections.ObjectModel;
using Sphera.API.Partners.DTOs;

namespace Sphera.API.Clients.DTOs;

public class ClientWithPartnerDTO : ClientDTO
{
    public PartnerDTO Partner { get; private set; }
    public ClientWithPartnerDTO(Guid id, string tradeName, string legalName, string cnpj, string stateRegistration, string municipalRegistration, 
        AddressDTO address, short? billingDueDay, bool status, DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy, 
        ReadOnlyCollection<ContactDTO> contacts, PartnerDTO partner) : base(id, tradeName, legalName, cnpj, stateRegistration, municipalRegistration, address, 
            billingDueDay, status, createdAt, createdBy, updatedAt, updatedBy, contacts)
    {
        Partner = partner;
    }
}
