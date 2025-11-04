namespace Sphera.API.Clients.DeleteClient;

/// <summary>
/// Represents a command to delete a client identified by a unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the client to be deleted.</param>
public record DeleteClientCommand(Guid Id);