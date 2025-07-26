using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs", "public");

        builder.HasKey(a => a.AuditLogId);

        builder.Property(a => a.AuditLogId)
               .HasColumnName("audit_log_id")
               .ValueGeneratedOnAdd();

        builder.Property(a => a.TableName)
               .HasColumnName("table_name")
               .HasMaxLength(100);

        builder.Property(a => a.Action)
               .HasColumnName("action")
               .HasMaxLength(50);

        builder.Property(a => a.KeyValues)
               .HasColumnName("key_values")
               .HasColumnType("text");

        builder.Property(a => a.OldValues)
               .HasColumnName("old_values")
               .HasColumnType("text");

        builder.Property(a => a.NewValues)
               .HasColumnName("new_values")
               .HasColumnType("text");

        builder.Property(a => a.ChangedBy)
               .HasColumnName("changed_by")
               .HasMaxLength(100);

        builder.Property(a => a.ChangedAt)
               .HasColumnName("changed_at")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
