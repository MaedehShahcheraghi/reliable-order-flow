using MassTransit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Order.Infrastructure.Sagas.Configurations;

public class OrderSagaStateMap : SagaClassMap<OrderSagaState>
{
     protected override void Configure(
        EntityTypeBuilder<OrderSagaState> entity,
        ModelBuilder model)
    {
        entity.ToTable(
            "order_saga_states");


        entity.Property(x => x.CurrentState)
            .HasMaxLength(64)
            .IsRequired();


        entity.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();


        entity.Property(
            x => x.InventoryReservationId);


        entity.Property(x => x.FailureReason)
            .HasMaxLength(500);


        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();


        entity.Property(
            x => x.FinishedAtUtc);
    }
}
