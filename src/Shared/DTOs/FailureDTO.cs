namespace Sphera.API.Shared.DTOs;

public sealed record FailureDTO(int Code, string Message)
{
    public static FailureDTO NotFound => new(404, "Resource not found");
    public static FailureDTO ValidationError => new(400, "Validation error occurred");
    public static FailureDTO Unauthorized => new(401, "Unauthorized access");
    public static FailureDTO InternalServerError => new(500, "An internal server error occurred");
}