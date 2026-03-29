using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(x => x.Id);

        //Propriedades
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(x => x.AccountId)
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CategoryId)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        builder.Property(x => x.NextDueDate)
            .IsRequired()
            .HasColumnType("datetime");
        
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnType("bit");

        builder.Property(x => x.Frequency)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("nvarchar(20)");
        
        //index
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new {x.UserId, x.IsActive});
        builder.HasIndex(x => x.NextDueDate);
        
        //relationship
        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}