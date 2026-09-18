using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderEntity = Order.Domain.Entities.Order;

namespace Order.Infrastructure.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{


    public DbSet<OrderEntity> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<OrderEntity>(order =>
        {
            order.ToTable("orders");

            order.HasKey(x => x.Id);


            order.Property(x => x.CustomerId)
            .IsRequired();


            order.Property(x => x.ProductId)
                .IsRequired();


            order.Property(x => x.Quantity)
                .IsRequired();


            order.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();


            order.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();


            order.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddInboxStateEntity();
    }
}

