using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Infrastructure.TableConfiguration;
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", "public");

        builder.HasKey(c => c.CustomerId);
        builder.Property(c => c.CustomerId).HasColumnName("customer_id");

        builder.Property(c => c.CustomerName)
            .HasColumnName("customer_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Phone).HasColumnName("phone");
        builder.Property(c => c.Address).HasColumnName("address");
        builder.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}