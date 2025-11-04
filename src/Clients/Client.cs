using Sphera.API.Partners;
using Sphera.API.Shared;
using Sphera.API.Shared.Contacts;
using Sphera.API.Shared.Contacts.Enums;
using Sphera.API.Shared.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphera.API.Clients;

public class Client
{
    [Key] public Guid Id { get; private set; }
    public Guid PartnerId { get; private set; }
    public string TradeName { get; private set; }
    public string LegalName { get; private set; }
    public CnpjValueObject Cnpj { get; private set; }
    public string StateRegistration { get; private set; }
    public string MunicipalRegistration { get; private set; }
    public AddressValueObject Address { get; private set; }
    public short? BillingDueDay { get; private set; }
    public bool Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public byte[] RowVersion { get; private set; }

    [ForeignKey(nameof(PartnerId))] public virtual Partner Partner { get; private set; }
    public virtual ICollection<Contact> Contacts { get; private set; } = new List<Contact>();

    private Client()
    {
    }

    public Client(Guid id, Guid partnerId, string tradeName, string legalName, CnpjValueObject? cnpj,
        AddressValueObject? address, Guid createdBy, short? billingDueDay = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        if (partnerId == Guid.Empty) throw new DomainException("PartnerId obrigatório.");
        PartnerId = partnerId;
        SetBasicInfo(tradeName, legalName, cnpj, address, billingDueDay);
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Status = true;
    }

    public void SetBasicInfo(string tradeName, string legalName, CnpjValueObject? cnpj, AddressValueObject? address,
       short? billingDueDay)
    {
        if (string.IsNullOrWhiteSpace(tradeName)) throw new DomainException("Nome fantasia obrigatório.");
        TradeName = tradeName;
        LegalName = legalName;
        Cnpj = cnpj ?? throw new DomainException("CNPJ obrigatório.");
        Address = address ?? throw new DomainException("Endereço obrigatório.");
        BillingDueDay = billingDueDay;
    }

    public void UpdateBasicInfo(string tradeName, string legalName, CnpjValueObject? cnpj,
        AddressValueObject? address, short? billingDueDay,  Guid actor)
    {
        SetBasicInfo(tradeName, legalName, cnpj, address, billingDueDay);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void UpdateAddress(AddressValueObject? address, Guid actor)
    {
        Address = address ?? throw new DomainException("Endereço inválido.");
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void Activate(Guid actor)
    {
        Status = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void Deactivate(Guid actor)
    {
        Status = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public Contact AddContact(
        EContactType contactType,
        EContactRole contactRole,
        string value,
        Guid actorId)
    {
        var contact = new Contact(contactType, contactRole, value, actorId, null, Id);
        Contacts.Add(contact);
        return contact;
    }

    public void RemoveContact(Guid contactId)
    {
        var contact = Contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is not null)
            Contacts.Remove(contact);
    }
}