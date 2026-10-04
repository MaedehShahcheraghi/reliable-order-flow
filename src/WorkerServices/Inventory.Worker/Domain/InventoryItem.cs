namespace Inventory.Worker.Domain;

public class InventoryItem
{
    public Guid ProductId { get; }
    public int OnHandQuantity { get;}
    public int ReservedQuantity { get; private set;}
   public int AvailableQuantity =>
        OnHandQuantity - ReservedQuantity;

    public uint Version { get; private set; }

    private InventoryItem() { }

      public InventoryItem(
        Guid productId,
        int onHandQuantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(onHandQuantity);

        ProductId = productId;

        OnHandQuantity = onHandQuantity;

        ReservedQuantity = 0;
    }

       public bool CanReserve(int quantity)
    {
        return quantity > 0 &&
               AvailableQuantity >= quantity;
    }


    public void Reserve(int quantity)
    {
        if (!CanReserve(quantity))
        {
            throw new InvalidOperationException(
                "Insufficient inventory.");
        }


        ReservedQuantity += quantity;
    }


    public void Release(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        if (ReservedQuantity < quantity)
        {
            throw new InvalidOperationException(
                "Cannot release more inventory than reserved.");
        }


        ReservedQuantity -= quantity;
    }
}


