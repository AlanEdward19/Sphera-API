using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Users;

namespace Sphera.API.External.Database.Maps;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");

        b.HasKey(u => u.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(u => u.RoleId)
            .HasColumnType("smallint")
            .IsRequired();

        b.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        b.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Address)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(160);

            email.HasIndex(u => u.Address)
                .HasDatabaseName("IX_User_Email")
                .IsUnique();
        });

        b.OwnsOne(u => u.Password, password =>
        {
            password.Property(p => p.Value)
                .HasColumnName("Password")
                .IsRequired()
                .HasMaxLength(200);
        });

        b.Property(u => u.Active)
            .IsRequired();

        b.Property(u => u.IsFirstAccess)
            .IsRequired();

        b.Property(u => u.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(u => u.UpdatedAt)
            .HasColumnType("datetime2");

        b.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_User_Role");
    }
}