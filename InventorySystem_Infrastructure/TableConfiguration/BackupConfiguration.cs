using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;
public class BackupConfiguration : IEntityTypeConfiguration<Backup>
{
    public void Configure(EntityTypeBuilder<Backup> builder)
    {
        builder.ToTable("backup");

        builder.HasKey(b => b.BackupId);

        builder.Property(b => b.BackupId)
            .HasColumnName("backup_id")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.BackupDate)
            .HasColumnName("backup_date")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(b => b.BackupStatus)
            .HasColumnName("backup_status")
            .IsRequired();

        builder.Property(c => c.IsActive)
           .HasColumnName("is_active")
           .HasDefaultValue(false);

        builder.Property(b => b.ErrorMessage)
            .HasColumnName("error_message");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(b => b.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.HasOne(b => b.CreatedByUser)
            .WithMany() // Or .WithMany(u => u.Backups) if reverse nav is needed
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("fk_backup_created_by");
    }
}
