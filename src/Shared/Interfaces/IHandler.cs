using Sphera.API.Shared.DTOs;

namespace Sphera.API.Shared.Interfaces;

public interface IHandler<TRequest, TResponse>
{
    Task<ResultDTO<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
