using Order.Infrastructure.Data;
using OrderProcessing.Contracts.Events;
using MassTransit;

namespace Order.Worker.Consumers;

public class PaymentCompletedConsumer(OrderDbContext orderDbContext) : IConsumer<PaymentCompleted>
{
    public Task Consume(ConsumeContext<PaymentCompleted> context)
    {
              var message =
            context.Message;
        var order = orderDbContext.Orders.Find(message.OrderId);
        if (order is not null)
        {
            order.MarkAsPaid();
            orderDbContext.SaveChanges();
        }

        return Task.CompletedTask;

    }

}