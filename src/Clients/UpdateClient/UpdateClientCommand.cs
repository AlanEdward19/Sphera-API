using Sphera.API.Shared.DTOs;

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
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the trade name associated with the entity.
    /// </summary>
    public string TradeName { get; set; }

    /// <summary>
    /// Gets or sets the registered legal name of the entity.
    /// </summary>
    public string LegalName { get; set; }

    /// <summary>
    /// Gets or sets the CNPJ (Cadastro Nacional da Pessoa Jurídica) number associated with the entity.
    /// </summary>
    /// <remarks>The CNPJ is a unique identifier assigned to legal entities in Brazil. The value should be a
    /// valid CNPJ format, typically consisting of 14 numeric digits. Callers are responsible for ensuring the value is
    /// correctly formatted and valid according to Brazilian regulations.</remarks>
    public string Cnpj { get; set; }

    /// <summary>
    /// Gets or sets the state registration identifier associated with the entity.
    /// </summary>
    public string StateRegistration { get; set; }

    /// <summary>
    /// Gets or sets the municipal registration number associated with the entity.
    /// </summary>
    public string MunicipalRegistration { get; set; }

    /// <summary>
    /// Gets or sets the address information associated with the entity.
    /// </summary>
    public AddressDTO Address { get; set; }

    /// <summary>
    /// Gets or sets the email address used for financial communications.
    /// </summary>
    public string FinancialEmail { get; set; }

    /// <summary>
    /// Gets or sets the phone number for financial inquiries.
    /// </summary>
    public string FinancialPhone { get; set; }

    /// <summary>
    /// Gets or sets the email address associated with the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number associated with the entity.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the day of the month on which billing is due.
    /// </summary>
    /// <remarks>A value between 1 and 31 represents the specific day of the month when payment is expected.
    /// If the value is null, no billing due day is set.</remarks>
    public short? BillingDueDay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current entity is active.
    /// </summary>
    public bool Status { get; set; } = true;
}
