namespace OrderProcessing.Contracts.Commands;

public sealed record ReleaseInventory
{
    public Guid OrderId { get; init; }

    public Guid ReservationId { get; init; }
}
