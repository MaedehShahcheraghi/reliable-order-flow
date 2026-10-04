namespace OrderProcessing.Contracts.Commands;

public sealed record ReserveInventory
{
    public Guid OrderId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}
