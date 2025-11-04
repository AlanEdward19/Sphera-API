using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.Shared.DTOs;

/// <summary>
/// Represents a data transfer object for an address, containing street, number, city, state, and zip code information.
/// </summary>
/// <remarks>This class is typically used to transfer address data between application layers or services. It does
/// not contain validation or business logic and is intended for use as a simple container for address-related
/// information.</remarks>
public class AddressDTO
{
    /// <summary>
    /// Gets or sets the street address component of the location.
    /// </summary>
    public string Street { get; set; }
    
    /// <summary>
    /// Gets or sets the numeric value associated with this instance.
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// Gets or sets the name of the city associated with the entity.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the current state of the object.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the postal code for the associated address.
    /// </summary>
    public string ZipCode { get; set; }

    /// <summary>
    /// Creates a new value object that represents the current address.
    /// </summary>
    /// <returns>An <see cref="AddressValueObject"/> instance containing the street, number, city, state, and zip code of the
    /// current address.</returns>
    public AddressValueObject ToValueObject() => new AddressValueObject(Street, Number, City, State, ZipCode);
}
