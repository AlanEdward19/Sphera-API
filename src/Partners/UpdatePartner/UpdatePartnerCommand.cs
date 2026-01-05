using Sphera.API.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Partners.UpdatePartner;

/// <summary>
/// Represents a command to update the details of a partner entity, including identification, legal name, CNPJ, address,
/// and status information.
/// </summary>
/// <remarks>This type is typically used to encapsulate the data required for updating an existing partner record
/// in the system. All properties should be set with valid values before submitting the command. The CNPJ property
/// should conform to Brazilian legal entity identification standards. The Address property allows for updating location
/// details associated with the partner.</remarks>
public class UpdatePartnerCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    private Guid Id { get; set; }

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
    /// Gets or sets a value indicating whether the current entity is active.
    /// </summary>
    public bool Status { get; set; } = true;

    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    /// <returns>A <see cref="System.Guid"/> value that uniquely identifies this instance.</returns>
    public Guid GetId() => Id;

    /// <summary>
    /// Sets the unique identifier for the current instance.
    /// </summary>
    /// <param name="id">The unique identifier to assign to the instance.</param>
    public void SetId(Guid id) => Id = id;
}
