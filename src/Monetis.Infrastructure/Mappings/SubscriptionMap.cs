using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Mappings;

public class SubscriptionMap : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId");
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime");
        
        builder.Property(x => x.AccountId)
            .IsRequired()
            .HasColumnName("AccountId");
        
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnName("Amount")
            .HasColumnType("decimal(18,2)");
        
        builder.Property(x => x.CategoryId)
            .IsRequired()
            .HasColumnName("CategoryId");
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasColumnName("Description")
            .HasMaxLength(50)
            .HasColumnType("nvarchar");

        builder.Property(x => x.NextDueDate)
            .IsRequired()
            .HasColumnName("NextDueDate")
            .ValueGeneratedNever()
            .HasColumnType("datetime");
        
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName("IsActive")
            .HasColumnType("bit");

        builder.Property(x => x.Frequency)
            .IsRequired()
            .HasColumnName("Frequency")
            .HasConversion<string>();

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
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
    }
}