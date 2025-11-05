namespace Sphera.API.Partners.GetPartnerById;

public record GetPartnerByIdQuery(Guid Id, bool? includeClients = false);
