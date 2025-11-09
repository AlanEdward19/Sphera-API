namespace Sphera.API.Shared.Interfaces;

public interface IHandler<in TRequest, TResponse>
{
    Task<IResultDTO<TResponse>> HandleAsync(TRequest request, HttpContext context, CancellationToken cancellationToken);
}