using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", "public");

        builder.HasKey(o => o.OrderItemId);
        builder.Property(o => o.OrderItemId).HasColumnName("order_item_id");

        builder.Property(o => o.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(o => o.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(o => o.SerialNo).HasColumnName("serial_no");

        builder.Property(o => o.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(o => o.UnitPrice).HasColumnName("unit_price").HasColumnType("numeric(18,2)");
        builder.Property(o => o.DiscountPercent).HasColumnName("discount_percent").HasDefaultValue(0);
        builder.Property(o => o.SubTotal).HasColumnName("sub_total");
        builder.Property(o => o.DiscountAmount).HasColumnName("discount_amount");
        builder.Property(o => o.NetTotal).HasColumnName("net_total");

        builder.Property(o => o.CreatedAt)
               .HasColumnName("created_at")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(o => o.CreatedBy).HasColumnName("created_by");

        builder.HasOne(o => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(o => o.OrderId)
            .HasConstraintName("fk_order_items_orders")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(o => o.ProductId)
            .HasConstraintName("fk_order_items_product")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.CreatedUser)
            .WithMany(u => u.CreatedOrderItems)
            .HasForeignKey(o => o.CreatedBy)
            .HasConstraintName("fk_order_items_users")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}
