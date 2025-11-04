namespace Sphera.API.Shared.ValueObjects;

public sealed record CnpjValueObject
{
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

    private static string Normalize(string? v) => new((v ?? "").Where(char.IsDigit).ToArray());

    public override string ToString() => Value;
}