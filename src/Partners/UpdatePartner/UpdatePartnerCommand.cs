using Sphera.API.Shared.DTOs;

namespace Sphera.API.Partners.UpdatePartner;

/// <summary>
/// Represents a command to update the details of a partner entity, including identification, registration, address, and
/// billing information.
/// </summary>
/// <remarks>This class is typically used to encapsulate the data required to update an existing partner's
/// information in the system. All properties should be set to valid values according to business and regulatory
/// requirements. Callers are responsible for ensuring that required fields are populated and that registration numbers,
/// such as CNPJ, conform to the expected formats.</remarks>
public class UpdatePartnerCommand
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
    public string? StateRegistration { get; set; }

    /// <summary>
    /// Gets or sets the municipal registration number associated with the entity.
    /// </summary>
    public string? MunicipalRegistration { get; set; }

    /// <summary>
    /// Gets or sets the address information associated with the entity.
    /// </summary>
    public AddressDTO Address { get; set; }

    /// <summary>
    /// Gets or sets the day of the month on which billing is due.
    /// </summary>
    /// <remarks>A value between 1 and 31 typically represents the calendar day of the month when payment is
    /// expected. If the value is null, no specific billing due day is set.</remarks>
    public short? BillingDueDay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current entity is active.
    /// </summary>
    public bool Status { get; set; } = true;
}
