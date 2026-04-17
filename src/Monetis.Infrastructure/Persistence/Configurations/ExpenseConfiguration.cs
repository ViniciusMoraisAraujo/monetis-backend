using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CategoryId)
            .IsRequired();

        builder.Property(e => e.SubscriptionId)
            .IsRequired(false);
        
        builder.Property(e => e.DueDate)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasColumnType("nvarchar(20)");

        builder.Property(e => e.PaidAt)
            .IsRequired(false)
            .HasColumnType("datetime");

        builder.Property(e => e.IsInstallment)
            .IsRequired()
            .HasColumnType("bit");

        builder.Property(e => e.InstallmentNumber)
            .IsRequired(false);

        builder.Property(e => e.TotalInstallments)
            .IsRequired(false);

        builder.Property(e => e.InstallmentGroupId)
            .IsRequired(false);

        builder.Property(e => e.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("nvarchar(20)");

        builder.Property(e => e.CreditCardId)
            .IsRequired(false);

        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.CreditCardId);
        builder.HasIndex(e => e.InstallmentGroupId);
        builder.HasIndex(x => x.AccountId);
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Card>()
            .WithMany()
            .HasForeignKey(e => e.CreditCardId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Subscription)
            .WithMany(s => s.GeneratedExpenses)
            .HasForeignKey(e => e.SubscriptionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
