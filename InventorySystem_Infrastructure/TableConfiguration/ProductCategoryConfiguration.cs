using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("product_category");

        builder.HasKey(pc => pc.ProductCategoryId);

        builder.Property(pc => pc.ProductCategoryId)
            .HasColumnName("product_category_id");

        builder.Property(pc => pc.ProductCategoryName)
            .HasColumnName("product_category_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pc => pc.CategoryId)
                .HasColumnName("category_id");

        builder.Property(pc => pc.Description)
            .HasColumnName("description");

        builder.Property(pc => pc.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(false);

        builder.Property(pc => pc.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(pc => pc.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(pc => pc.ModifiedAt)
            .HasColumnName("modified_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(pc => pc.ModifiedBy)
            .HasColumnName("modified_by");

        // Foreign Keys
        builder.HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId)
            .HasConstraintName("fk_productcategory_category");

        builder.HasOne(pc => pc.CreatedByUser)
            .WithMany()
            .HasForeignKey(pc => pc.CreatedBy)
            .HasConstraintName("fk_productcategory_createdby");

        builder.HasOne(pc => pc.ModifiedByUser)
            .WithMany()
            .HasForeignKey(pc => pc.ModifiedBy)
            .HasConstraintName("fk_productcategory_modifiedby");

        builder.HasMany(pc => pc.Products)
            .WithOne(p => p.ProductCategory)
            .HasForeignKey(p => p.ProductCategoryId)
            .HasConstraintName("fk_productcategory_category");

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}
