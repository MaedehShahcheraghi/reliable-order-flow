using MassTransit;
using OrderProcessing.Contracts.Events;
using Payment.Worker.Data;
using Payment.Worker.Domain;
using Payment.Worker.Domain.Enums;
using PaymentEntity = Payment.Worker.Domain.Payment;
namespace Payment.Worker.Consumers;

public class OrderSubmittedConsumer(PaymentDbContext paymentDbContext, IPublishEndpoint publishEndpoint) : IConsumer<OrderSubmitted>
{
    public Task Consume(ConsumeContext<OrderSubmitted> context)
    {
              var message =
            context.Message;

        var payment= new PaymentEntity(message.OrderId, message.TotalAmount, PaymentStatus.Completed);
        paymentDbContext.Payments.Add(payment);

        publishEndpoint.Publish(new PaymentCompleted
        {
            OrderId = message.OrderId,
            PaymentId = payment.Id,
            Amount = payment.Amount,
            CompletedAtUtc = DateTime.UtcNow
        });
        paymentDbContext.SaveChanges();
        return Task.CompletedTask;

    }

}
