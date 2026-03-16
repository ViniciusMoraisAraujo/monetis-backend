using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Persistence.Configurations;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        //Propriedades
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(160)
            .HasColumnType("nvarchar(160)");

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime");
        
        builder.HasIndex(x => x.Email).IsUnique();
        
    }
}