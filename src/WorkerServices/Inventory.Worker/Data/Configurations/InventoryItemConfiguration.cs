using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inventory.Worker.Domain;

namespace Inventory.Worker.Data.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
            builder.ToTable("inventory_items");

            builder.HasKey(x => x.ProductId);

            builder.Property(x => x.OnHandQuantity)
                .IsRequired();

            builder.Property(x => x.ReservedQuantity)
                .IsRequired();

            builder.Property(x => x.Version)
            .IsRowVersion();

            builder.Ignore(x => x.AvailableQuantity);
    }

}
