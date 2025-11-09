namespace Sphera.API.Services.DeleteService;

/// <summary>
/// Represents a command to delete a service identified by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the service to be deleted.</param>
public record DeleteServiceCommand(Guid Id);