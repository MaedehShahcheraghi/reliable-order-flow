using Order.Domain.Entities.Eums;

namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; }

    public Guid CustomerId { get; }

    public Guid ProductId { get; }

    public int Quantity { get; }

    public decimal TotalAmount { get; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; }


    private Order()
    {
    }


    public Order(
        Guid customerId,
        Guid productId,
        int quantity,
        decimal totalAmount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalAmount);

        Id = Guid.NewGuid();

        CustomerId = customerId;

        ProductId = productId;

        Quantity = quantity;

        TotalAmount = totalAmount;

        Status = OrderStatus.Submitted;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        Status = OrderStatus.Paid;
    }

    public void MarkPaymentAsFailed()
    {
        Status = OrderStatus.failed;
    }
}
