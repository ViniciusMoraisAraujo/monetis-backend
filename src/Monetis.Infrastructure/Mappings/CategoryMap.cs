using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Mappings;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnType("datetime");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30)
            .HasColumnType("nvarchar(30)");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("nvarchar(20)");

        builder.Property(x => x.UserId)
            .ValueGeneratedNever();

        builder.Property(x => x.Icon)
            .IsRequired()
            .HasColumnType("nvarchar(250)");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}