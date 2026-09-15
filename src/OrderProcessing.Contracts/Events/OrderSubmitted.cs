namespace OrderProcessing.Contracts.Events;

public class OrderSubmitted
{
        public Guid OrderId { get; init; }

    public Guid CustomerId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public decimal TotalAmount { get; init; }

    public DateTime SubmittedAtUtc { get; init; }
}
