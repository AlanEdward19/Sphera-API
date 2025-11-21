using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Contacts;

namespace Sphera.API.External.Database.Maps;

/// <summary>
/// Configures the entity mapping for the Contact entity type in the Entity Framework model.
/// </summary>
/// <remarks>This configuration defines table mapping, property constraints, relationships, and indexes for the
/// Contact entity. It is typically used within the Entity Framework Core model building process to ensure the Contact
/// entity is correctly mapped to the database schema.</remarks>
public class ContactMap : IEntityTypeConfiguration<Contact>
{
    /// <summary>
    /// Configures the entity type mapping for the Contact entity.
    /// </summary>
    /// <remarks>This method defines the table mapping, property configurations, relationships, and indexes
    /// for the Contact entity. It should be called within the Entity Framework Core model configuration
    /// process.</remarks>
    /// <param name="b">The builder used to configure the Contact entity type.</param>
    public void Configure(EntityTypeBuilder<Contact> b)
    {
        b.ToTable("Contacts");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");
        
        b.Property(x => x.Name)
            .HasMaxLength(160);

        b.Property(x => x.Type)
            .IsRequired();

        b.Property(x => x.Role)
            .IsRequired();
        
        b.Property(x => x.PhoneType);

        b.Property(x => x.Value)
            .HasMaxLength(160)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(x => x.CreatedBy)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2");

        b.Property(x => x.UpdatedBy)
            .HasColumnType("uniqueidentifier");

        b.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        b.HasOne(x => x.Partner)
            .WithMany(p => p.Contacts)
            .HasForeignKey(x => x.PartnerId)
            .HasConstraintName("FK_Contacts_Partner")
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.Client)
            .WithMany(c => c.Contacts)
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_Contacts_Client")
            .OnDelete(DeleteBehavior.NoAction);
        
        b.HasOne(x => x.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_Contacts_User")
            .OnDelete(DeleteBehavior.NoAction);

        b.HasIndex(x => x.PartnerId).HasDatabaseName("IX_Contacts_PartnerId");
        b.HasIndex(x => x.ClientId).HasDatabaseName("IX_Contacts_ClientId");
        b.HasIndex(x => x.UserId).HasDatabaseName("IX_Contacts_UserId");
        b.HasIndex(x => x.Role).HasDatabaseName("IX_Contacts_Role");
    }
}
