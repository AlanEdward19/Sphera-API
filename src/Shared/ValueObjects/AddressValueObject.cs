using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Shared.ValueObjects;

public sealed record AddressValueObject
{
    /// <summary>
    /// Gets the street component of the address.
    /// </summary>
    public string Street { get; init; }

    /// <summary>
    /// Gets the number associated with this instance.
    /// </summary>
    public int? Number { get; init; }

    /// <summary>
    /// Gets the name of the city associated with the entity.
    /// </summary>
    public string City { get; init; }
    
    /// <summary>
    /// Gets the two-letter postal abbreviation for the state.
    /// </summary>
    /// <remarks>The value must be exactly two characters in length, typically conforming to standard U.S.
    /// state abbreviations (e.g., "NY" for New York).</remarks>
    [MinLength(2)]
    [MaxLength(2)]
    public string State { get; init; }

    /// <summary>
    /// Gets the postal code associated with the address.
    /// </summary>
    public string ZipCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the AddressValueObject class with the specified address details.
    /// </summary>
    /// <param name="street">The street name of the address. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="number">The street number of the address. Can be null if the number is not applicable.</param>
    /// <param name="city">The city of the address. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <param name="state">The two-letter state abbreviation. Must not be null, empty, and must consist of exactly two characters.</param>
    /// <param name="zipCode">The postal code for the address. Can be null or empty if not applicable.</param>
    /// <exception cref="DomainException">Thrown if street or city is null, empty, or consists only of white-space characters, or if state is null, empty,
    /// or not exactly two characters.</exception>
    public AddressValueObject(string street, int? number, string city, string state, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new DomainException("Rua é obrigatória.");
        if (string.IsNullOrWhiteSpace(city)) throw new DomainException("Cidade é obrigatória.");
        if (string.IsNullOrWhiteSpace(state) || state.Length != 2) throw new DomainException("Estado inválido.");
        Street = street;
        Number = number;
        City = city;
        State = state.ToUpperInvariant();
        ZipCode = zipCode;
    }
}