using Order.Infrastructure.Sagas;
using MassTransit;
using OrderProcessing.Contracts.Events;
using OrderProcessing.Contracts.Commands;
namespace Order.Worker.Sagas;

public class OrderStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State WaitingForInventory { get; private set; }
        = default!;


    public Event<OrderSubmitted> Submitted { get; private set; }
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
