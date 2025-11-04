using Sphera.API.Shared.DTOs;

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
}
