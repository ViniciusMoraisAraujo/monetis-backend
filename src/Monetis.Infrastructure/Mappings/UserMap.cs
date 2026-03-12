using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monetis.Domain.Entities;

namespace Monetis.Infrastructure.Mappings;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        //Propriedades

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasColumnName("FirstName")
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("Id");


        builder.Property(x => x.LastName)
            .IsRequired()
            .HasColumnName("LastName")
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        builder.Property(x => x.Email)
            .IsRequired()
            .HasColumnName("Email")
            .HasMaxLength(160)
            .HasColumnType("nvarchar(160)");

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasColumnName("PasswordHash")
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime");
        
        builder.HasIndex(x => x.Email, "IX_Users_Email").IsUnique();
        
    }
}