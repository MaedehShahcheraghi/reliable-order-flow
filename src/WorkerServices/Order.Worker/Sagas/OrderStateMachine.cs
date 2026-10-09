using Order.Infrastructure.Sagas;
using MassTransit;
using OrderProcessing.Contracts.Events.Inventory;
using OrderProcessing.Contracts.Events;
using OrderProcessing.Contracts.Commands;
namespace Order.Worker.Sagas;

public class OrderStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State WaitingForInventory { get; private set; }
        = default!;

    public State WaitingForPayment { get; private set; }
    = default!;

    public State Cancelled { get; private set; }
    = default!;
    public Event<OrderSubmitted> Submitted { get; private set; }
        = default!;

        public Event<InventoryReserved>
    InventoryReservedEvent { get; private set; }
    = default!;


    public Event<InventoryReservationFailed>
    InventoryReservationFailedEvent { get; private set; }
    = default!;

      public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted, x =>
        {
            x.CorrelateById(context => context.Message.OrderId);
            x.InsertOnInitial = true;
            x.SelectId(context => context.Message.OrderId);
        });

        Event(() => InventoryReservedEvent, x =>
{
    x.CorrelateById(
        context => context.Message.OrderId);
});


Event(() => InventoryReservationFailedEvent, x =>
{
    x.CorrelateById(
        context => context.Message.OrderId);
});
        Initially(
            When(Submitted)
                .Then(context =>
                {
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.CreatedAtUtc = DateTime.UtcNow;
                }).Send(new Uri("queue:inventory-reserve"), context => new ReserveInventory
                {
                        OrderId =
                            context.Message.OrderId,

                        ProductId =
                            context.Message.ProductId,

                        Quantity =
                            context.Message.Quantity
                })
                .TransitionTo(WaitingForInventory));
    }
}
