using Order.Infrastructure.Data;
using OrderProcessing.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
namespace Order.Worker.Consumers;

public sealed class PaymentFailedConsumer(
    OrderDbContext orderDbContext)
    : IConsumer<PaymentFailed>
{
    public async Task Consume(
        ConsumeContext<PaymentFailed> context)
    {
        var message = context.Message;


        var order =
            await orderDbContext.Orders
                .SingleOrDefaultAsync(
                    x => x.Id == message.OrderId,
                    context.CancellationToken) ?? throw new InvalidOperationException(
                $"Order {message.OrderId} was not found.");

        order.MarkPaymentAsFailed();


        await orderDbContext.SaveChangesAsync(
            context.CancellationToken);
    }
}
