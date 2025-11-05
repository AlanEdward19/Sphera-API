using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sphera.API.Shared.ValueObjects;
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
        
        var emailConverter = new ValueConverter<EmailValueObject, string>(
            v => v.Address,
            v => new EmailValueObject(v)
        );
        
        var emailComparer = new ValueComparer<EmailValueObject>(
            (a, b) => a.Address == b.Address,
            v => v == null ? 0 : v.Address.GetHashCode(),
            v => new EmailValueObject(v.Address)
        );

        var emailProp = b.Property(u => u.Email);

        emailProp
            .HasConversion(emailConverter)
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(160);
        
        emailProp.Metadata.SetValueComparer(emailComparer);
        
        var passwordConverter = new ValueConverter<PasswordValueObject, string>(
            v => v.Value,
            v => new PasswordValueObject(v)
        );
        
        var passwordComparer = new ValueComparer<PasswordValueObject>(
            (a, b) => a.Value == b.Value,
            v => v == null ? 0 : v.Value.GetHashCode(),
            v => new PasswordValueObject(v.Value)
        );
        
        var passwordProp = b.Property(u => u.Password);
        
        passwordProp.HasConversion(passwordConverter)
            .HasColumnName("Password")
            .IsRequired()
            .HasMaxLength(200);
        
        passwordProp.Metadata.SetValueComparer(passwordComparer);

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