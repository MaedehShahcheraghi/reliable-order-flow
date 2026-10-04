namespace OrderProcessing.Contracts.Events.Inventory;

public sealed record InventoryReleased
{
    public Guid OrderId { get; init; }

    public Guid ReservationId { get; init; }

    public DateTime ReleasedAtUtc { get; init; }
}