namespace OrderProcessing.Contracts.Commands;

public sealed record ProcessPayment
{
    public Guid OrderId { get; init; }

    public decimal Amount { get; init; }
}
