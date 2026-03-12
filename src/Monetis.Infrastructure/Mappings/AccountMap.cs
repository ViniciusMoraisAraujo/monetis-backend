using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Mappings;

public class AccountMap : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Account");
        builder.HasKey(x => x.Id);

        //property
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("Id");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasMaxLength(25)
            .HasColumnType("varchar(25)");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId");

        builder.Property(x => x.Balance)
            .IsRequired()
            .HasColumnName("Balance");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasColumnName("Type");

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasColumnName("Currency");

        //relationship
        builder.
            HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}