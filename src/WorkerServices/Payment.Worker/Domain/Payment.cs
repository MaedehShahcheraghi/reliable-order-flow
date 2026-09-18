using Payment.Worker.Domain.Enums;
namespace Payment.Worker.Domain;

public class Payment
{
     public Guid Id { get;  }

    public Guid OrderId { get;}

    public decimal Amount { get; }

    public PaymentStatus Status { get;}

    public DateTime CreatedAtUtc { get; }

    private Payment()
    {

    }

    public Payment(Guid orderId,decimal amount,PaymentStatus paymentStatus)
    {
        OrderId = orderId;
        Amount = amount;
        Status=paymentStatus;
    }
}
