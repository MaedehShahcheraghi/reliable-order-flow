using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PaymetEntity= Payment.Worker.Domain.Payment;

namespace Payment.Worker.Data;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{

    public DbSet<PaymetEntity> Payments => Set<PaymetEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var payment =
            modelBuilder.Entity<PaymetEntity>();

        payment.ToTable("payments");

        payment.HasKey(x => x.Id);

        payment.Property(x => x.OrderId)
            .IsRequired();

        payment.Property(x => x.Amount)
            .HasPrecision(18, 2);

        payment.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        payment.Property(x => x.CreatedAtUtc)
            .IsRequired();

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

}
