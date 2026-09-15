using MassTransit;
using OrderProcessing.Contracts.Events;

namespace Payment.Worker.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    public Task Consume(ConsumeContext<OrderSubmitted> context)
    {
              var message =
            context.Message;


        Console.WriteLine();
        Console.WriteLine(
            "========== PAYMENT ==========");

        Console.WriteLine(
            $"OrderId: {message.OrderId}");

        Console.WriteLine(
            $"Amount: {message.TotalAmount}");

        Console.WriteLine(
            $"MessageId: {context.MessageId}");

        Console.WriteLine(
            $"CorrelationId: {context.CorrelationId}");

        Console.WriteLine(
            $"Payment received OrderSubmitted ,Tracing number: {context.CorrelationId}");

        Console.WriteLine(
            "=============================");


        return Task.CompletedTask;

    }

}
