using Sphera.API.Clients.DTOs;
using Sphera.API.Contacts;
using Sphera.API.Shared.DTOs;
using System.Collections.ObjectModel;

namespace Sphera.API.Partners.DTOs;

public class PartnerWithClientsDTO : PartnerDTO
{
    public ReadOnlyCollection<ClientDTO> Clients { get; private set; }

    public PartnerWithClientsDTO(Guid id, string legalName, string cnpj, AddressDTO address, bool status,
        DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy, ReadOnlyCollection<ContactDTO> contacts,
        ReadOnlyCollection<ClientDTO> clients)
        : base(id, legalName, cnpj, address, status, createdAt, createdBy, updatedAt, updatedBy, contacts)
    {
        Clients = clients;
    }
}
