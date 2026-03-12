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
            .HasMaxLength(30)
            .HasColumnType("nvarchar(30)");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasColumnName("Type");

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .ValueGeneratedNever();

        builder.Property(x => x.Icon)
            .IsRequired()
            .HasColumnName("Icon")
            .HasColumnType("nvarchar(250)");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}