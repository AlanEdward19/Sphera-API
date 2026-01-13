using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Billets.DownloadBilletFile;

public class DownloadBilletFileCommandHandler([FromKeyedServices("billing")] IStorage storage) : IHandler<DownloadBilletFileCommand, (Stream, string)>
{
    public Task<IResultDTO<(Stream, string)>> HandleAsync(DownloadBilletFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

