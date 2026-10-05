using MassTransit;

namespace Order.Infrastructure.Sagas;

public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }
        = default!;


    public decimal TotalAmount { get; set; }


    public Guid? InventoryReservationId { get; set; }


    public string? FailureReason { get; set; }


    public DateTime CreatedAtUtc { get; set; }


    public DateTime? FinishedAtUtc { get; set; }
}
