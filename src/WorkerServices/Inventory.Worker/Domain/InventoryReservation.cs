using Inventory.Worker.Domain.Enums;
namespace Inventory.Worker.Domain;

public sealed class InventoryReservation
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public InventoryReservationStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReleasedAtUtc { get; private set; }


    private InventoryReservation()
    {
    }


    public InventoryReservation(
        Guid orderId,
        Guid productId,
        int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity));

        Id = Guid.NewGuid();

        OrderId = orderId;

        ProductId = productId;

        Quantity = quantity;

        Status =
            InventoryReservationStatus.Reserved;

        CreatedAtUtc =
            DateTime.UtcNow;
    }


    public void Release()
    {
        if (Status ==
            InventoryReservationStatus.Released)
            return;

        Status =
            InventoryReservationStatus.Released;

        ReleasedAtUtc =
            DateTime.UtcNow;
    }
}
