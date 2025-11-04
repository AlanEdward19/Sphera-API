namespace Sphera.API.Shared.DTOs;

public sealed class ResultDTO<T>
{
    public T? Success { get; private set; }
    public FailureDTO? Failure { get; private set; }
    public bool IsSuccess => Failure == null;
    public bool IsFailure => !IsSuccess;

    private ResultDTO(T success) { Success = success; }
    private ResultDTO(FailureDTO failure) { Failure = failure; }
    public static ResultDTO<T> AsSuccess(T success) => new(success);
    public static ResultDTO<T> AsFailure(FailureDTO failure) => new(failure);
}