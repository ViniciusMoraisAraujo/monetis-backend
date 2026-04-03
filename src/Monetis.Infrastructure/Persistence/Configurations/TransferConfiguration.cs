using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("Transfers");

        builder.Property(t => t.DestinationAccountId)
            .IsRequired();

        builder.Property(t => t.TransferredAt)
            .IsRequired()
            .HasColumnType("datetime");

        builder.HasIndex(t => t.DestinationAccountId);

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
        
        builder.HasOne(t => t.DestinationAccount)
            .WithMany()
            .HasForeignKey(t => t.DestinationAccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        
    }
}
