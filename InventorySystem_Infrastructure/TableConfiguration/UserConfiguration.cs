using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Infrastructure.TableConfiguration;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");

        entity.HasKey(e => e.UserId);

        entity.Property(e => e.UserId)
              .HasColumnName("user_id")
              .ValueGeneratedOnAdd();

        entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(30).IsRequired();
        entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(30);
        entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(30).IsRequired();
        entity.HasIndex(e => e.UserName).IsUnique();

        entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(256).IsRequired();
        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(150);
        entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
        entity.Property(e => e.PasswordLastChanged).HasColumnName("password_last_changed").IsRequired();

        entity.Property(e => e.PasswordExpiresAt)
              .HasColumnName("password_expires_at")
              .ValueGeneratedOnAddOrUpdate()
              .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

        entity.Property(e => e.IsPasswordExpired).HasColumnName("is_password_expired").IsRequired();
        entity.Property(e => e.LastLogin).HasColumnName("last_login");
        entity.Property(e => e.MobileNo).HasColumnName("mobile_no").HasMaxLength(10).IsRequired();
        entity.Property(e => e.ProfileImage).HasColumnName("profile_image");

        entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        entity.Property(e => e.CreatedDate).HasColumnName("created_date").IsRequired();
        entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
        entity.Property(c => c.RowVersion).IsRowVersion();
    }
}