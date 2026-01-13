using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Remittances.DownloadRemittanceFile;

public class DownloadRemittanceFileCommandHandler([FromKeyedServices("billing")] IStorage storage) : IHandler<DownloadRemittanceFileCommand, (Stream, string)>
{
    public Task<IResultDTO<(Stream, string)>> HandleAsync(DownloadRemittanceFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}