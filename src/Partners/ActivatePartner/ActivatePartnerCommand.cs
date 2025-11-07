namespace Sphera.API.Partners.ActivatePartner;

/// <summary>
/// Represents a request to activate a partner identified by a unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the partner to activate.</param>
public record ActivatePartnerCommand(Guid Id);
