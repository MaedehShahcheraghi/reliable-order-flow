namespace OrderProcessing.Contracts.Events.Payment;

public sealed record PaymentCompleted
{
    public Guid OrderId { get; init; }

    public Guid PaymentId { get; init; }

    public decimal Amount { get; init; }

    public DateTime CompletedAtUtc { get; init; }
}
