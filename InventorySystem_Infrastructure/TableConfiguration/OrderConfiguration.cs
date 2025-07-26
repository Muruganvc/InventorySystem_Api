using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", "public");

        builder.HasKey(o => o.OrderId);
        builder.Property(o => o.OrderId).HasColumnName("order_id");

        builder.Property(o => o.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(o => o.OrderDate).HasColumnName("order_date")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(o => o.TotalAmount).HasColumnName("total_amount").HasColumnType("numeric(18,2)");
        builder.Property(o => o.FinalAmount).HasColumnName("final_amount").HasColumnType("numeric(18,2)");
        builder.Property(o => o.BalanceAmount).HasColumnName("balance_amount").HasColumnType("numeric(18,2)");
        builder.Property(o => o.IsGst).HasColumnName("is_gst").HasDefaultValue(false);
        builder.Property(o => o.GstNumber).HasColumnName("gst_number").HasMaxLength(15);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .HasConstraintName("fk_orders_customer")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}
