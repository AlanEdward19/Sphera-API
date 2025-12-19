namespace Sphera.API.Billing.BillingEntries.Common;

public sealed class FailureItemDTO
{
    public Guid Id { get; init; }
    public string Error { get; init; } = string.Empty;

    public FailureItemDTO() { }
    public FailureItemDTO(Guid id, string error)
    {
        Id = id;
        Error = error;
    }
}

public sealed class BulkActionResultDTO
{
    public IReadOnlyCollection<Guid> SucceededIds { get; }
    public IReadOnlyCollection<FailureItemDTO> Failures { get; }

    public BulkActionResultDTO(IEnumerable<Guid> succeededIds, IEnumerable<FailureItemDTO> failures)
    {
        SucceededIds = succeededIds.Distinct().ToList().AsReadOnly();
        Failures = failures.ToList().AsReadOnly();
    }
}

