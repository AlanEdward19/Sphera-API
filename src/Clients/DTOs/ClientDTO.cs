using Sphera.API.Contacts;
using Sphera.API.Shared.DTOs;
using System.Collections.ObjectModel;
using Sphera.API.Shared.Enums;

namespace Sphera.API.Clients.DTOs;

public class ClientDTO
{
    public Guid Id { get; private set; }
    public string TradeName { get; private set; }
    public string LegalName { get; private set; }
    public string Cnpj { get; private set; }
    public string? StateRegistration { get; private set; }
    public string? MunicipalRegistration { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? EcacExpirationDate { get; private set; }
    public AddressDTO Address { get; private set; }
    public short? BillingDueDay { get; private set; }
    public DateTime? ContractDate { get; private set; }
    public EExpirationStatus? ExpirationStatus { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public ReadOnlyCollection<ContactDTO> Contacts { get; private set; }
    public int DocumentsCount { get; private set; }

    public ClientDTO(Guid id, string tradeName, string legalName, string cnpj, string? stateRegistration,
        string? municipalRegistration, AddressDTO address, short? billingDueDay, DateTime? contractDate,
        EExpirationStatus? expirationStatus, bool status,
        DateTime createdAt, Guid createdBy, DateTime? updatedAt, Guid? updatedBy,
        ReadOnlyCollection<ContactDTO> contacts, int documentsCount, string? notes = null,
        DateTime? ecacExpirationDate = null)
    {
        Id = id;
        TradeName = tradeName;
        LegalName = legalName;
        Cnpj = cnpj;
        StateRegistration = stateRegistration;
        MunicipalRegistration = municipalRegistration;
        Notes = notes;
        EcacExpirationDate = ecacExpirationDate;
        Address = address;
        BillingDueDay = billingDueDay;
        ContractDate = contractDate;
        ExpirationStatus = expirationStatus;
        Status = status;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
        Contacts = contacts;
        DocumentsCount = documentsCount;
    }
}