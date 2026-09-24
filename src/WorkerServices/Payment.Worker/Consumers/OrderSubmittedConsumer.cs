using MassTransit;
using OrderProcessing.Contracts.Events;
using Payment.Worker.Data;
using Payment.Worker.Domain;
using Payment.Worker.Domain.Enums;
using Payment.Worker.GatewaySimulation;
using PaymentEntity = Payment.Worker.Domain.Payment;

namespace Payment.Worker.Consumers;

public sealed class OrderSubmittedConsumer(
    PaymentDbContext paymentDbContext,
    IPaymentGateway paymentGateway)
    : IConsumer<OrderSubmitted>
{
    public async Task Consume(
        ConsumeContext<OrderSubmitted> context)
    {
        var message = context.Message;


        var result =
            await paymentGateway.ChargeAsync(
                message.OrderId,
                message.TotalAmount,
                context.CancellationToken);


        if (!result.IsSuccessful)
        {
            await HandleFailedPayment(
                context,
                message,
                result);

            return;
        }


        await HandleSuccessfulPayment(
            context,
            message);
    }


    private async Task HandleSuccessfulPayment(
        ConsumeContext<OrderSubmitted> context,
        OrderSubmitted message)
    {
        var payment =
            new PaymentEntity(
                message.OrderId,
                message.TotalAmount,
                PaymentStatus.Completed);


        paymentDbContext.Payments.Add(payment);


        await context.Publish(
            new PaymentCompleted
            {
                OrderId = message.OrderId,
                PaymentId = payment.Id,
                Amount = payment.Amount,
                CompletedAtUtc = DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    context.CorrelationId
                    ?? message.OrderId;
            });


        await paymentDbContext.SaveChangesAsync(
            context.CancellationToken);
    }


    private async Task HandleFailedPayment(
        ConsumeContext<OrderSubmitted> context,
        OrderSubmitted message,
        PaymentGatewayResult result)
    {
        var payment =
            new PaymentEntity(
                message.OrderId,
                message.TotalAmount,
                PaymentStatus.Failed);


        paymentDbContext.Payments.Add(payment);


        await context.Publish(
            new PaymentFailed
            {
                OrderId = message.OrderId,
                PaymentId = payment.Id,
                Amount = payment.Amount,

                Reason =
                    result.FailureReason
                    ?? "Payment declined",

                FailedAtUtc =
                    DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    context.CorrelationId
                    ?? message.OrderId;
            });


        await paymentDbContext.SaveChangesAsync(
            context.CancellationToken);
    }
}