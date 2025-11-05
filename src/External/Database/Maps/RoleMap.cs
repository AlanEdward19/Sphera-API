using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Roles;

namespace Sphera.API.External.Database.Maps;

public class RoleMap : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles");

        b.HasKey(r => r.Id);

        b.Property(r => r.Id)
            .HasColumnType("smallint")
            .ValueGeneratedOnAdd();

        b.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(r => r.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(r => r.UpdatedAt)
            .HasColumnType("datetime2");
    }
}