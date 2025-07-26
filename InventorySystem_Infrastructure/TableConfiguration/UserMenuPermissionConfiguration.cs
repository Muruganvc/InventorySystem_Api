using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class UserMenuPermissionConfiguration : IEntityTypeConfiguration<UserMenuPermission>
{
    public void Configure(EntityTypeBuilder<UserMenuPermission> builder)
    {
        builder.ToTable("user_menu_permissions");

        builder.HasKey(x => x.UserMenuPermissionId);

        builder.Property(x => x.UserMenuPermissionId)
            .HasColumnName("user_menu_permission_id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.MenuItemId)
            .HasColumnName("menu_item_id")
            .IsRequired();

        builder.Property(x => x.OrderBy)
            .HasColumnName("order_by")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.MenuItemId })
            .IsUnique()
            .HasDatabaseName("uq_user_menu");

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserMenuPermissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MenuItem)
            .WithMany(m => m.UserMenuPermissions)
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}