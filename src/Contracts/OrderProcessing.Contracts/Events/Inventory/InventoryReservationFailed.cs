namespace OrderProcessing.Contracts.Events.Inventory;

public sealed record InventoryReservationFailed
{
    public Guid OrderId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public string Reason { get; init; } = default!;

    public DateTime FailedAtUtc { get; init; }
}