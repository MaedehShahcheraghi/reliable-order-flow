using Inventory.Worker.Data;
using Inventory.Worker.Domain.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Contracts.Commands;
using OrderProcessing.Contracts.Events.Inventory;

namespace Inventory.Worker.Consumers;

public class ReleaseInventoryConsumer(InventoryDbContext dbContext) : IConsumer<ReleaseInventory>
{
    public async Task Consume(ConsumeContext<ReleaseInventory> context)
    {
         var message = context.Message;


        var reservation =
            await dbContext.Reservations
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == message.ReservationId &&
                        x.OrderId == message.OrderId,
                    context.CancellationToken) ?? throw new InvalidOperationException(
                $"Inventory reservation {message.ReservationId} " +
                $"for order {message.OrderId} was not found.");


        if (reservation.Status ==
            InventoryReservationStatus.Released)
        {
            return;
        }


        var inventory =
            await dbContext.InventoryItems
                .SingleOrDefaultAsync(
                    x => x.ProductId == reservation.ProductId,
                    context.CancellationToken);


        if (inventory is null)
        {
            throw new InvalidOperationException(
                $"Inventory item {reservation.ProductId} was not found.");
        }


        inventory.Release(
            reservation.Quantity);


        reservation.Release();


        await context.Publish(
            new InventoryReleased
            {
                OrderId =
                    message.OrderId,

                ReservationId =
                    reservation.Id,

                ReleasedAtUtc =
                    DateTime.UtcNow
            },
            publishContext =>
            {
                publishContext.CorrelationId =
                    message.OrderId;
            });

    }

}
