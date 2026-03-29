using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {

        builder.Property(e => e.DueDate)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(e => e.PaidAt)
            .IsRequired(false);
    }
}