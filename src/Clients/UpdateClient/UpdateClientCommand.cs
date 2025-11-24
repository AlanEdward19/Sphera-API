using Sphera.API.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Clients.UpdateClient;

/// <summary>
/// Represents a command to update the details of a client entity, including identification, registration, contact, and
/// billing information.
/// </summary>
/// <remarks>This class is typically used in scenarios where client information needs to be modified or maintained
/// in a system, such as updating legal or trade names, registration numbers, contact details, or billing preferences.
/// All properties should be set with valid and up-to-date information to ensure data integrity. Callers are responsible
/// for validating property values, such as ensuring the CNPJ is correctly formatted according to Brazilian regulations
/// and that the billing due day, if specified, falls within the valid range for days of the month.</remarks>
public class UpdateClientCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    private Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the trade name associated with the entity.
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(160)]
    public string TradeName { get; set; }

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
    /// Gets or sets the phone number for financial inquiries.
    /// </summary>
    [Required]
    [Phone]
    public string FinancialPhone { get; set; }

    /// <summary>
    /// Gets or sets the email address associated with the user.
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
    /// <remarks>A value between 1 and 31 represents the specific day of the month when payment is expected.
    /// If the value is null, no billing due day is set.</remarks>
    [Range(1, 31)]
    public short? BillingDueDay { get; set; }
    
    public short? ContractDateInDays { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current entity is active.
    /// </summary>
    public bool Status { get; set; } = true;

    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    /// <returns>A <see cref="System.Guid"/> that uniquely identifies this instance.</returns>
    public Guid GetId() => Id;

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => Id = id;
}
