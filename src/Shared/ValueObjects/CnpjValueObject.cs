namespace Sphera.API.Shared.ValueObjects;

/// <summary>
/// Represents a CNPJ (Cadastro Nacional da Pessoa Jurídica) value object containing a validated 14-digit CNPJ number.
/// </summary>
/// <remarks>The CNPJ is a unique identifier assigned to legal entities in Brazil. This value object ensures that
/// the CNPJ is normalized to a 14-digit string and validated for correct length and digit-only content upon creation.
/// Use this type to encapsulate and validate CNPJ values throughout your application.</remarks>
public sealed record CnpjValueObject
{
    /// <summary>
    /// Gets the CNPJ value as a string of 14 digits.
    /// </summary>
    public string Value { get; init; }

    public CnpjValueObject(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CNPJ é obrigatório.");

        var normalized = Normalize(value);
        if (normalized.Length != 14 || !normalized.All(char.IsDigit))
            throw new DomainException("CNPJ inválido. Deve conter 14 dígitos.");

        // TODO: implementar validação de dígito verificador
        Value = normalized;
    }

    /// <summary>
    /// Extracts and returns a string containing only the digit characters from the specified input.
    /// </summary>
    /// <param name="v">The input string to normalize. If null, an empty string is used.</param>
    /// <returns>A string consisting of all digit characters found in the input, in their original order. Returns an empty string
    /// if the input is null or contains no digits.</returns>
    private static string Normalize(string? v) => new((v ?? "").Where(char.IsDigit).ToArray());

    /// <summary>
    /// Returns the string representation of the current object.
    /// </summary>
    /// <returns>A string that represents the value of the current object.</returns>
    public override string ToString() => Value;
}