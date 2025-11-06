using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Shared.DTOs;

public sealed class ResultDTO<T> : IResultDTO<T>
{
    public T? Success { get; private set; }
    public FailureDTO? Failure { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure { get; private set; }

    private ResultDTO() { }

    public static ResultDTO<T> AsSuccess(T success) =>
        new ResultDTO<T> { Success = success, IsSuccess = true, IsFailure = false };

    public static ResultDTO<T> AsFailure(FailureDTO failure) =>
        new ResultDTO<T> { Failure = failure, IsSuccess = false, IsFailure = true };
}