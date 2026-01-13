using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Remittances.GenerateRemittanceFile;

public class GenerateRemittanceFileCommandHandler([FromKeyedServices("billing")] IStorage storage) : IHandler<GenerateRemittanceFileCommand, bool>
{
    public Task<IResultDTO<bool>> HandleAsync(GenerateRemittanceFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

