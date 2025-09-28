using InventorySystem_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySystem_Infrastructure.TableConfiguration;

public class PaymentHistoryConfiguration : IEntityTypeConfiguration<PaymentHistory>
{
    public void Configure(EntityTypeBuilder<PaymentHistory> builder)
    {
        // Table name and check constraint
        builder.ToTable("payment_history", t =>
        {
            t.HasCheckConstraint("CK_payment_method",
                "payment_method IN ('Cash Payments', 'Cheque Payments', 'Online Payments')");
        });

        // Primary Key
        builder.HasKey(ph => ph.PaymentHistoryId);

        // Properties
        builder.Property(ph => ph.PaymentHistoryId)
            .HasColumnName("payment_history_id");

        builder.Property(ph => ph.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(ph => ph.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(ph => ph.AmountPaid)
            .HasColumnName("amount_paid")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(ph => ph.PaymentAt)
            .HasColumnName("payment_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ph => ph.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(50);

        builder.Property(ph => ph.TransactionRefNo)
            .HasColumnName("transaction_ref_no")
            .HasMaxLength(100);

        builder.Property(ph => ph.BalanceRemainingToPay)
            .HasColumnName("balance_remaining_to_pay")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(ph => ph.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        // Relationships (Foreign Keys)
        builder.HasOne(ph => ph.Order)
            .WithMany()
            .HasForeignKey(ph => ph.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ph => ph.Customer)
            .WithMany()
            .HasForeignKey(ph => ph.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ph => ph.CreatedByUser)
            .WithMany()
            .HasForeignKey(ph => ph.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
