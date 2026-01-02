using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sphera.API.Contacts;
using Sphera.API.Partners;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.External.Database.Maps;

/// <summary>
/// Configures the entity mapping for the Partner type in the Entity Framework model.
/// </summary>
/// <remarks>This configuration defines how the Partner entity is mapped to the database, including table name,
/// primary key, property constraints, owned types, indexes, and relationships. It should be used with Entity Framework
/// Core's model builder to ensure the Partner entity is correctly represented in the database schema.</remarks>
public class PartnerMap : IEntityTypeConfiguration<Partner>
{
    /// <summary>
    /// Configures the entity type mapping for the Partner entity using the specified builder.
    /// </summary>
    /// <remarks>This method defines table mapping, property constraints, indexes, owned types, and
    /// relationships for the Partner entity. It should be called within the Entity Framework Core model configuration
    /// process to ensure the Partner entity is correctly mapped to the database schema.</remarks>
    /// <param name="b">The builder used to configure the Partner entity type and its properties.</param>
    public void Configure(EntityTypeBuilder<Partner> b)
    {
        b.ToTable("Partners");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.LegalName).HasMaxLength(160).IsRequired();

        var cnpjConverter = new ValueConverter<CnpjValueObject, string>(
            v => v.Value,
            v => new CnpjValueObject(v)
        );

        var cnpjComparer = new ValueComparer<CnpjValueObject>(
            (a, b) => a.Value == b.Value,
            v => v == null ? 0 : v.Value.GetHashCode(),
            v => new CnpjValueObject(v.Value)
        );

        var cnpjProp = b.Property(c => c.Cnpj);

        cnpjProp.HasConversion(cnpjConverter)
                .HasColumnName("Cnpj")
                .HasMaxLength(14);

        cnpjProp.Metadata.SetValueComparer(cnpjComparer);

        b.Property(x => x.Notes).HasMaxLength(500);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasIndex(x => x.Cnpj)
            .HasDatabaseName("IX_Partners_Cnpj")
            .IsUnique();

        b.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(160);
            a.Property(p => p.Number).HasColumnName("Number");
            a.Property(p => p.Complement).HasColumnName("Complement").HasMaxLength(120);
            a.Property(p => p.Neighborhood).HasColumnName("Neighborhood").HasMaxLength(100);
            a.Property(p => p.City).HasColumnName("City").HasMaxLength(100);
            a.Property(p => p.State).HasColumnName("State").HasMaxLength(2);
            a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(10);
            a.Property(p => p.Lot).HasColumnName("Lot").HasMaxLength(40);
        });

        b.HasMany(p => p.Clients)
            .WithOne(ct => ct.Partner)
            .HasForeignKey(c => c.PartnerId)
            .HasConstraintName("FK_Contacts_Partner")
            .OnDelete(DeleteBehavior.NoAction);

        b.HasMany(c => c.Contacts)
            .WithOne(ct => ct.Partner)
            .HasForeignKey(ct => ct.PartnerId)
            .HasConstraintName("FK_Contacts_Partner")
            .OnDelete(DeleteBehavior.NoAction);
    }
}