using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");

        builder.HasKey(m => m.MenuItemId);

        builder.Property(m => m.MenuItemId)
               .HasColumnName("menu_items_id")
               .ValueGeneratedOnAdd();

        builder.Property(m => m.Label)
               .HasColumnName("label")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(m => m.Icon)
               .HasColumnName("icon")
               .HasMaxLength(50);

        builder.Property(m => m.Route)
               .HasColumnName("route")
               .HasMaxLength(200);

        builder.Property(m => m.ParentId)
               .HasColumnName("parent_id");

        builder.Property(m => m.OrderBy)
               .HasColumnName("order_by")
               .HasDefaultValue(0);

        builder.HasOne(m => m.Parent)
               .WithMany(m => m.Children)
               .HasForeignKey(m => m.ParentId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}