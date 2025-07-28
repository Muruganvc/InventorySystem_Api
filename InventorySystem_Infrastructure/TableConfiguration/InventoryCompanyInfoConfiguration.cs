using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Infrastructure.TableConfiguration;
public class InventoryCompanyInfoConfiguration : IEntityTypeConfiguration<InventoryCompanyInfo>
{
    public void Configure(EntityTypeBuilder<InventoryCompanyInfo> builder)
    {
        builder.ToTable("inventory_company_info");

        builder.HasKey(e => e.InventoryCompanyInfoId);

        builder.Property(e => e.InventoryCompanyInfoId)
               .HasColumnName("inventory_company_info_id")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.InventoryCompanyInfoName)
               .HasColumnName("inventory_company_info_name")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(e => e.Description)
               .HasColumnName("description")
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(e => e.Address)
               .HasColumnName("address")
               .HasMaxLength(250)
               .IsRequired();

        builder.Property(e => e.MobileNo)
               .HasColumnName("mobile_no")
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(e => e.Email)
               .HasColumnName("email")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(e => e.GstNumber)
               .HasColumnName("gst_number")
               .HasMaxLength(15)
               .IsRequired();

        builder.Property(e => e.BankName)
               .HasColumnName("bank_name")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(e => e.BankBranchName)
               .HasColumnName("bank_branch_name")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(e => e.BankAccountNo)
               .HasColumnName("bank_account_no")
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(e => e.BankBranchIFSC)
               .HasColumnName("bank_branch_ifsc")
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(e => e.ApiVersion)
               .HasColumnName("api_version")
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(e => e.UiVersion)
               .HasColumnName("ui_version")
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(e => e.QrCode)
               .HasColumnName("qr_code")
               .IsRequired(false);

        builder.Property(c => c.IsActive)
       .HasColumnName("is_active")
       .HasDefaultValue(false);

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}