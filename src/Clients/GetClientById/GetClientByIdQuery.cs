namespace Sphera.API.Clients.GetClientById;

public record GetClientByIdQuery(Guid Id, bool? includePartner);