using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("category");

        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
               .HasColumnName("category_id")
               .ValueGeneratedOnAdd();

        builder.Property(c => c.CategoryName)
               .HasColumnName("category_name")
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .HasColumnName("description");

        builder.Property(c => c.IsActive)
               .HasColumnName("is_active")
               .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
               .HasColumnName("created_at")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.CreatedBy)
               .HasColumnName("created_by")
               .IsRequired();

        builder.Property(c => c.ModifiedAt)
               .HasColumnName("modified_at")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.ModifiedBy)
               .HasColumnName("modified_by");

        builder.Property(c => c.CompanyId)
               .HasColumnName("company_id");

        // Foreign key to Company
 

        builder.HasOne(c => c.Company)
            .WithMany(c => c.Categories)
            .HasForeignKey(c => c.CompanyId)
            .HasConstraintName("fk_category_company");

        // Foreign key to CreatedBy User
        builder.HasOne(c => c.CreatedByUser)
               .WithMany()
               .HasForeignKey(c => c.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_category_createdby");

        // Foreign key to ModifiedBy User
        builder.HasOne(c => c.ModifiedByUser)
               .WithMany()
               .HasForeignKey(c => c.ModifiedBy)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_category_modifiedby");

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}
