using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.Property(t => t.TransferredAt)
            .IsRequired();
        
        builder.HasOne(t => t.DestinationAccount)
            .WithMany()
            .HasForeignKey(t => t.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}