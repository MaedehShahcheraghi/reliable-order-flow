using Inventory.Worker.Data;
using Inventory.Worker.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Contracts.Commands;
using OrderProcessing.Contracts.Events.Inventory;

namespace Inventory.Worker.Consumers;

public class ReserveInventoryConsumer(InventoryDbContext dbContext) : IConsumer<ReserveInventory>
{
    public async Task Consume(ConsumeContext<ReserveInventory> context)
    {
         var message = context.Message;

        var inventory =
            await dbContext.InventoryItems
                .SingleOrDefaultAsync(
                    x => x.ProductId == message.ProductId,
                    context.CancellationToken);

        if (inventory is null)
        {
            await PublishFailed(
                context,
                "Product was not found.");

            return;
        }

        if (!inventory.CanReserve(message.Quantity))
        {
            await PublishFailed(
                context,
                "Insufficient inventory.");

            return;
        }


        inventory.Reserve(
            message.Quantity);


        var reservation =
            new InventoryReservation(
                message.OrderId,
                message.ProductId,
                message.Quantity);


        dbContext.Reservations.Add(
            reservation);


        await context.Publish(
            new InventoryReserved
            {
                OrderId =
                    message.OrderId,

                ReservationId =
                    reservation.Id,

                ProductId =
                    message.ProductId,

                Quantity =
                    message.Quantity,

                ReservedAtUtc =
                    DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    message.OrderId;
            });


        await dbContext.SaveChangesAsync(
            context.CancellationToken);
    }


    private static Task PublishFailed(
        ConsumeContext<ReserveInventory> context,
        string reason)
    {
        return context.Publish(
            new InventoryReservationFailed
            {
                OrderId =
                    context.Message.OrderId,

                ProductId =
                    context.Message.ProductId,

                Quantity =
                    context.Message.Quantity,

                Reason = reason,

                FailedAtUtc =
                    DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    context.Message.OrderId;
            });
    }
}
