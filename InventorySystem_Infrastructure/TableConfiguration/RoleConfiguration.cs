using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
               .HasColumnName("role_id")
               .ValueGeneratedOnAdd();

        builder.Property(r => r.RoleName)
               .HasColumnName("role_name")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(r => r.RoleCode)
               .HasColumnName("role_code")
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(r => r.RoleName).IsUnique();
        builder.HasIndex(r => r.RoleCode).IsUnique();

        builder.Property(r => r.IsActive)
               .HasColumnName("is_active")
               .IsRequired()
               .HasDefaultValue(true);
    }
}