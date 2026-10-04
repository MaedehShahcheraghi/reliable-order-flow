using MassTransit;
using Microsoft.EntityFrameworkCore;
using Inventory.Worker.Domain;

namespace Inventory.Worker.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
     public DbSet<InventoryItem> InventoryItems =>
        Set<InventoryItem>();

    public DbSet<InventoryReservation> Reservations =>
        Set<InventoryReservation>();
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();

        modelBuilder.AddOutboxMessageEntity();

        modelBuilder.AddOutboxStateEntity();
    }
}
