namespace Sphera.API.Services.DeleteService;

/// <summary>
/// Represents a command to delete a service identified by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the service to be deleted.</param>
public class DeleteServiceCommand(Guid id);