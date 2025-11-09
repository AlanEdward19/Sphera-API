namespace Sphera.API.Services.ActivateService;

/// <summary>
/// Represents a command to activate a service identified by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the service to activate.</param>
public record ActivateServiceCommand(Guid Id);