using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(x => x.Id);

        //property
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnType("datetime");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(25)
            .HasColumnType("nvarchar(25)");

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Balance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnType("nvarchar(25)");

        builder.HasIndex(x => x.UserId);
        
        //relationship
        builder.
            HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}