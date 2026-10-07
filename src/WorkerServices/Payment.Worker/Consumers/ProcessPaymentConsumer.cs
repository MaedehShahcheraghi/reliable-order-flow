using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Contracts.Commands;
using OrderProcessing.Contracts.Events.Payment;
using Payment.Worker.Data;
using Payment.Worker.Domain.Enums;
using Payment.Worker.GatewaySimulation;
using PaymentEntity = Payment.Worker.Domain.Payment;

namespace Payment.Worker.Consumers;

public sealed class ProcessPaymentConsumer(
    PaymentDbContext paymentDbContext,
    IPaymentGateway paymentGateway)
    : IConsumer<ProcessPayment>
{
    public async Task Consume(
        ConsumeContext<ProcessPayment> context)
    {
        var message = context.Message;

        var existingPayment =
            await paymentDbContext.Payments
                .SingleOrDefaultAsync(
                    x => x.OrderId == message.OrderId,
                    context.CancellationToken);


        if (existingPayment is not null)
        {
            return;
        }

        var result =
            await paymentGateway.ChargeAsync(
                message.OrderId,
                message.Amount,
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
        ConsumeContext<ProcessPayment> context,
        ProcessPayment message)
    {
        var payment =
            new PaymentEntity(
                message.OrderId,
                message.Amount,
                PaymentStatus.Completed);


        paymentDbContext.Payments.Add(payment);


        await context.Publish(
            new PaymentCompleted
            {
                OrderId =
                    message.OrderId,

                PaymentId =
                    payment.Id,

                Amount =
                    payment.Amount,

                CompletedAtUtc =
                    DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    message.OrderId;
            });


        await paymentDbContext.SaveChangesAsync(
            context.CancellationToken);
    }


    private async Task HandleFailedPayment(
        ConsumeContext<ProcessPayment> context,
        ProcessPayment message,
        PaymentGatewayResult result)
    {
        var payment =
            new PaymentEntity(
                message.OrderId,
                message.Amount,
                PaymentStatus.Failed);


        paymentDbContext.Payments.Add(payment);


        await context.Publish(
            new PaymentFailed
            {
                OrderId =
                    message.OrderId,

                PaymentId =
                    payment.Id,

                Amount =
                    payment.Amount,

                Reason =
                    result.FailureReason
                    ?? "Payment declined",

                FailedAtUtc =
                    DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    message.OrderId;
            });


        await paymentDbContext.SaveChangesAsync(
            context.CancellationToken);
    }
}