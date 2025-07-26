using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product", "public");

            // Primary key
            builder.HasKey(p => p.ProductId);
            builder.Property(p => p.ProductId)
                .HasColumnName("product_id")
                .ValueGeneratedOnAdd();

            // Basic properties
            builder.Property(p => p.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => p.ProductName).IsUnique();

            builder.Property(p => p.ProductCategoryId)
                .HasColumnName("product_category_id");

            builder.Property(p => p.Description)
                .HasColumnName("description");

            builder.Property(p => p.MRP)
                .HasColumnName("mrp")
                .HasColumnType("numeric(10,2)");

            builder.Property(p => p.SalesPrice)
                .HasColumnName("sales_price")
                .HasColumnType("numeric(10,2)");

            builder.Property(p => p.Quantity)
                .HasColumnName("quantity");

            builder.Property(p => p.LandingPrice)
                .HasColumnName("landing_price")
                .HasColumnType("numeric(18,2)")
                .HasDefaultValue(18.00m);

            builder.Property(p => p.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired();

            builder.Property(p => p.ModifiedAt)
                .HasColumnName("modified_at");

            builder.Property(p => p.ModifiedBy)
                .HasColumnName("modified_by");

            // Relationships
            builder.HasOne(p => p.ProductCategory)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .HasConstraintName("fk_product_category");

            builder.HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .HasConstraintName("fk_created_by");

            builder.HasOne(p => p.ModifiedByUser)
                .WithMany()
                .HasForeignKey(p => p.ModifiedBy)
                .HasConstraintName("fk_modified_by");

            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .HasConstraintName("fk_orderitem_product")
                .OnDelete(DeleteBehavior.Restrict); // or Cascade if your logic requires

            builder.Property(c => c.RowVersion).IsRowVersion();
        }
    }
}
