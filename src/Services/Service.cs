using System.ComponentModel.DataAnnotations;
using Sphera.API.Shared;

namespace Sphera.API.Services;

public class Service
{
    [Key]
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; } // único
    
    [MinLength(0)]
    public short DefaultDueInDays { get; private set; }
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public byte[] RowVersion { get; private set; }

    private Service() { }
    

    public Service(Guid id, string name, string code, short defaultDueInDays, Guid createdBy)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome do serviço obrigatório.");
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Código do serviço obrigatório.");
        if (defaultDueInDays < 0) throw new DomainException("DefaultDueInDays inválido.");
        Name = name;
        Code = code;
        DefaultDueInDays = defaultDueInDays;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(string name, short defaultDueInDays, Guid actor)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name;
        if (defaultDueInDays >= 0) DefaultDueInDays = defaultDueInDays;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actor;
    }

    public void Activate(Guid actor) { IsActive = true; UpdatedAt = DateTime.UtcNow; UpdatedBy = actor; }
    public void Deactivate(Guid actor) { IsActive = false; UpdatedAt = DateTime.UtcNow; UpdatedBy = actor; }
}
