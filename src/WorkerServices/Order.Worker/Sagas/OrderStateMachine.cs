using MassTransit;
using Order.Infrastructure.Sagas;
using OrderProcessing.Contracts.Commands;
using OrderProcessing.Contracts.Events;
using OrderProcessing.Contracts.Events.Inventory;
using OrderProcessing.Contracts.Events.Payment;
namespace Order.Worker.Sagas;

public class OrderStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State WaitingForInventory { get; private set; }
        = default!;

    public State WaitingForPayment { get; private set; }
    = default!;

    public State ReleasingInventory { get; private set; }
    = default!;

    public State Completed { get; private set; }
    = default!;

    public State Cancelled { get; private set; }
    = default!;
    public Event<OrderSubmitted> Submitted { get; private set; }
        = default!;

    public Event<InventoryReserved>
InventoryReservedEvent
    { get; private set; }
= default!;

    public Event<PaymentCompleted>
PaymentCompletedEvent
    { get; private set; }
= default!;

    public Event<PaymentFailed>
PaymentFailedEvent
    { get; private set; }
= default!;

    public Event<InventoryReleased> InventoryReleasedEvent { get; private set; } = default!;

    public Event<InventoryReservationFailed>
    InventoryReservationFailedEvent
    { get; private set; }
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

        Event(() => PaymentCompletedEvent, x =>
{
    x.CorrelateById(
        context =>
            context.Message.OrderId);
});


        Event(() => PaymentFailedEvent, x =>
        {
            x.CorrelateById(
                context =>
                    context.Message.OrderId);
        });


        Event(() => InventoryReleasedEvent, x =>
        {
            x.CorrelateById(
                context =>
                    context.Message.OrderId);
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
        During(
WaitingForInventory,

When(InventoryReservedEvent)
.Then(context =>
{
    context.Saga.InventoryReservationId =
        context.Message.ReservationId;
})
.Send(
    new Uri("queue:payment-process-queue"),
    context => new ProcessPayment
    {
        OrderId =
            context.Saga.CorrelationId,

        Amount =
            context.Saga.TotalAmount
    })
.TransitionTo(
    WaitingForPayment),
When(InventoryReservationFailedEvent).Then(context =>
{
    context.Saga.FailureReason = context.Message.Reason;
    context.Saga.FinishedAtUtc = DateTime.UtcNow;
}).TransitionTo(Cancelled));
        During(WaitingForPayment,
        When(PaymentCompletedEvent)
        .Then(context => context.Saga.FinishedAtUtc = DateTime.UtcNow)
        .TransitionTo(Completed),
        When(PaymentFailedEvent)
        .Then(context =>
        {
            context.Saga.FailureReason = context.Message.Reason;
        })
        .Send(new Uri("queue:inventory-release"), context => new ReleaseInventory
        {
            OrderId =
                context.Saga.CorrelationId,

            ReservationId =
                context.Saga.InventoryReservationId!.Value
        }).TransitionTo(ReleasingInventory)

        );
        During(
            ReleasingInventory,

            When(InventoryReleasedEvent)
                .Then(context =>
                {
                    if (context.Saga.InventoryReservationId
                        != context.Message.ReservationId)
                    {
                        throw new InvalidOperationException(
                            "Released inventory reservation " +
                            "does not match the saga reservation.");
                    }


                    context.Saga.FinishedAtUtc =
                        DateTime.UtcNow;
                })
                .TransitionTo(
                    Cancelled)
        );

    }
}
