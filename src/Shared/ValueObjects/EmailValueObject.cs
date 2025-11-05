using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Shared.ValueObjects;

public sealed record EmailValueObject
{
    [EmailAddress] 
    [MinLength(1)]
    [MaxLength(160)]
    public string Address { get; init; }

    public EmailValueObject(string address)
    {
        Address = address;
    }
}