using MassTransit;
using OrderProcessing.Contracts.Events;

namespace Order.Worker.Consumers;

public class PaymentFaultConsumer : IConsumer<Fault<OrderSubmitted>>
{
    public Task Consume(ConsumeContext<Fault<OrderSubmitted>> context)
    {
         var fault =
            context.Message;

        var exception =
            fault.Exceptions[0];

        return Task.CompletedTask;
    }

}
