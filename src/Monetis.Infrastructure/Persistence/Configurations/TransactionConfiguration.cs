using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();
        
        builder.HasDiscriminator<string>("TransactionType")
            .HasValue<Expense>("Expense")
            .HasValue<Income>("Income")
            .HasValue<Transfer>("Transfer");
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.AccountId)
            .IsRequired();
            
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

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
    }
}