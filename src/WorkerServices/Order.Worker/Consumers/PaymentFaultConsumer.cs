using MassTransit;
using OrderProcessing.Contracts.Commands;

namespace Order.Worker.Consumers;


public sealed class PaymentFaultConsumer
    : IConsumer<Fault<ProcessPayment>>
{
    public Task Consume(
        ConsumeContext<Fault<ProcessPayment>> context)
    {
        var fault =
            context.Message;

        var exception =
            fault.Exceptions.FirstOrDefault();

        Console.WriteLine(
            $"Payment processing faulted. " +
            $"OrderId={fault.Message.OrderId} | " +
            $"Exception={exception?.Message}");

        return Task.CompletedTask;
    }
}