namespace OrderProcessing.Contracts.Events.Inventory;

public sealed record InventoryReserved
{
    public Guid OrderId { get; init; }

    public Guid ReservationId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public DateTime ReservedAtUtc { get; init; }
}