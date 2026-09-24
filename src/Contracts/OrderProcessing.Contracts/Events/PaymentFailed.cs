namespace OrderProcessing.Contracts.Events;

public sealed record PaymentFailed
{ 
     public Guid OrderId { get; init; }

    public Guid PaymentId { get; init; }

    public decimal Amount { get; init; }

    public string Reason { get; init; } = default!;

    public DateTime FailedAtUtc { get; init; }
}
