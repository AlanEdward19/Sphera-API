using System.Text.RegularExpressions;

namespace Sphera.API.Shared.ValueObjects;

public sealed record PasswordValueObject
{
    public string Value { get; }
    
    public PasswordValueObject(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Senha não pode ser vazia.");

        if (value.Length < 8)
            throw new DomainException("Senha deve ter pelo menos 8 caracteres.");

        if (!Regex.IsMatch(value, "[A-Z]"))
            throw new DomainException("Senha deve conter pelo menos uma letra maiúscula.");

        if (!Regex.IsMatch(value, "[a-z]"))
            throw new DomainException("Senha deve conter pelo menos uma letra minúscula.");

        if (!Regex.IsMatch(value, "[0-9]"))
            throw new DomainException("Senha deve conter pelo menos um número.");

        if (!Regex.IsMatch(value, "[^a-zA-Z0-9]"))
            throw new DomainException("Senha deve conter pelo menos um caractere especial.");

        Value = value;
    }
    
    public override string ToString() => Value;
}