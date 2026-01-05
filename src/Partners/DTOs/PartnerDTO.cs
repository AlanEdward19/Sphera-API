using Sphera.API.Contacts;
using Sphera.API.Shared.DTOs;
using System.Collections.ObjectModel;

namespace Sphera.API.Partners.DTOs;

public class PartnerDTO
{
    public Guid Id { get; private set; }
    public string LegalName { get; private set; }
    public string Cnpj { get; private set; }
    public AddressDTO? Address { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public ReadOnlyCollection<ContactDTO> Contacts { get; private set; }
    public int ClientsCount { get; private set; }
    public string? Notes { get; private set; }

    public PartnerDTO(Guid id, string legalName, string cnpj, AddressDTO? address, bool status,
        DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy, ReadOnlyCollection<ContactDTO> contacts, int clientsCount, string? notes = null)
    {
        Id = id;
        LegalName = legalName;
        Cnpj = cnpj;
        Address = address;
        Status = status;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
        Contacts = contacts;
        ClientsCount = clientsCount;
        Notes = notes;
    }
}