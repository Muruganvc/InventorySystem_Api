using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("company");

        builder.HasKey(c => c.CompanyId);
        builder.Property(c => c.CompanyId)
            .HasColumnName("company_id")
            .UseIdentityAlwaysColumn();

        builder.Property(c => c.CompanyName)
            .HasColumnName("company_name")
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
            .HasColumnName("modified_by")
            .IsRequired(false);

        // Foreign keys (assuming Users table exists with UserId)

        builder.HasMany(c => c.Categories)
            .WithOne(cat => cat.Company)
            .HasForeignKey(cat => cat.CompanyId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_category_company");

        builder.HasOne(c => c.CreatedByUser)
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_company_created_by");

        builder.HasOne(c => c.ModifiedByUser)
            .WithMany()
            .HasForeignKey(c => c.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_company_modified_by");

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}