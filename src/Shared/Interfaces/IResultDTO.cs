using Sphera.API.Shared.DTOs;

namespace Sphera.API.Shared.Interfaces;

public interface IResultDTO<out T>
{
    T? Success { get; }
    FailureDTO? Failure { get; }
    bool IsSuccess { get; }
    bool IsFailure { get; }
}