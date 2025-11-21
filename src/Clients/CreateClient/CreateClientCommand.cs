using Sphera.API.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Clients.CreateClient;

/// <summary>
/// Represents a command to create a new client entity with associated identification, contact, and billing information.
/// </summary>
/// <remarks>This class is typically used to encapsulate the data required when registering a new client,
/// including legal identifiers, contact details, and billing preferences. All properties should be populated with valid
/// values according to their respective formats and business rules. No automatic validation or formatting is performed
/// by this class.</remarks>
public class CreateClientCommand
{
    /// <summary>
    /// Gets or sets the unique identifier of the partner associated with this entity.
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Gets or sets the trade name associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string TradeName { get; set; }

    /// <summary>
    /// Gets or sets the legal name of the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string LegalName { get; set; }

    /// <summary>
    /// Gets or sets the CNPJ (Cadastro Nacional da Pessoa Jurídica) number associated with the entity.
    /// </summary>
    /// <remarks>The CNPJ is a unique identifier assigned to legal entities in Brazil. The value should be a
    /// valid CNPJ format, typically consisting of 14 numeric digits. No formatting or validation is performed
    /// automatically.</remarks>
    [Required]
    [MinLength(14)]
    [MaxLength(14)]
    public string Cnpj { get; set; }

    /// <summary>
    /// Gets or sets the state registration identifier associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string StateRegistration { get; set; }

    /// <summary>
    /// Gets or sets the municipal registration number associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string MunicipalRegistration { get; set; }
    
    /// <summary>
    /// Gets or sets the address information associated with the entity.
    /// </summary>
    public AddressDTO Address { get; set; }

    /// <summary>
    /// Gets or sets the email address used for financial communications.
    /// </summary>
    [Required]
    [EmailAddress]
    public string FinancialEmail { get; set; }

    /// <summary>
    /// Gets or sets the phone number for financial contacts.
    /// </summary>
    [Required]
    [Phone]
    public string FinancialPhone { get; set; }

    /// <summary>
    /// Gets or sets the email address associated with the entity.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number associated with the entity.
    /// </summary>
    [Required]
    [Phone]
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the day of the month on which billing is due.
    /// </summary>
    /// <remarks>A value between 1 and 31 typically represents the calendar day of the month when payment is
    /// expected. If the value is null, no specific billing due day is set.</remarks>
    [Range(1, 31)]
    public short? BillingDueDay { get; set; }
    
    [Range(1, 31)]
    public short ContractDateInDays { get; set; }
}
