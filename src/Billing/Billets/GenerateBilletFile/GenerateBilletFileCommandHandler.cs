using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Billets.GenerateBilletFile;

public class GenerateBilletFileCommandHandler([FromKeyedServices("billing")] IStorage storage) : IHandler<GenerateBilletFileCommand, bool>
{
    public Task<IResultDTO<bool>> HandleAsync(GenerateBilletFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

