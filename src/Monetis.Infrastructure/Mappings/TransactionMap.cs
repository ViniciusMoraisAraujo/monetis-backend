using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Mappings;

public class TransactionMap : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .ValueGeneratedNever()
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.AccountId)
            .ValueGeneratedNever()
            .IsRequired();
            
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CategoryId)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");
            
        builder.Property(x => x.PaidAt)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnType("datetime");
        
        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasColumnType("nvarchar(20)");

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.PaidAt);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        

        
    }
}