using Sphera.API.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Partners.CreatePartner;

/// <summary>
/// Represents a command to create a new partner entity, including identification, registration, address, and billing
/// information.
/// </summary>
/// <remarks>This class is typically used to encapsulate the data required when registering a new partner in the
/// system. All required fields must be provided to ensure successful creation. The CNPJ property should be supplied in
/// a valid Brazilian CNPJ format. Optional properties, such as state and municipal registration or billing due day, may
/// be left unset if not applicable.</remarks>
public class CreatePartnerCommand
{
    /// <summary>
    /// Gets or sets the registered legal name of the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string LegalName { get; set; }

    /// <summary>
    /// Gets or sets the CNPJ (Cadastro Nacional da Pessoa Jurídica) number associated with the entity.
    /// </summary>
    /// <remarks>The CNPJ is a unique identifier assigned to legal entities in Brazil. The value should be a
    /// valid CNPJ format, typically consisting of 14 numeric digits. Callers are responsible for ensuring the value is
    /// correctly formatted and valid according to Brazilian regulations.</remarks>
    [MinLength(14)]
    [MaxLength(14)]
    public string? Cnpj { get; set; }

    /// <summary>
    /// Gets or sets the address information associated with the entity.
    /// </summary>
    public AddressDTO? Address { get; set; }

    /// <summary>
    /// Gets or sets the optional notes or comments associated with this instance.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the email address used for financial communications.
    /// </summary>
    [EmailAddress]
    public string? FinancialEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number for financial contacts.
    /// </summary>
    [Phone]
    public string? FinancialPhone { get; set; }
    
    /// <summary>
    /// Gets or sets the email address associated with the responsible party.
    /// </summary>
    [EmailAddress]
    public string? ResponsibleEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number associated with the responsible party.
    /// </summary>
    [Phone]
    public string? ResponsiblePhone { get; set; }
    
    /// <summary>
    /// Gets or sets the landline phone number associated with the entity.
    /// </summary>
    [Phone]
    public string? LandLine { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number associated with the entity.
    /// </summary>
    [Required]
    [Phone]
    public string Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the backup phone number associated with the entity.
    /// </summary>
    [Phone]
    public string? BackupPhone { get; set; }
}
