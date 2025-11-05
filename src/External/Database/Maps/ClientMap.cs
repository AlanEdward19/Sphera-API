using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sphera.API.Clients;
using Sphera.API.Contacts;
using Sphera.API.Partners;
using Sphera.API.Shared.ValueObjects;

namespace Sphera.API.External.Database.Maps;

/// <summary>
/// Configures the entity mapping for the Client type in the Entity Framework model.
/// </summary>
/// <remarks>This configuration defines how the Client entity is mapped to the database, including table name,
/// primary key, property constraints, indexes, owned types, and relationships. It should be used with Entity Framework
/// Core's model builder to ensure the Client entity is correctly represented in the database schema.</remarks>
public class ClientMap : IEntityTypeConfiguration<Client>
{
    /// <summary>
    /// Configures the entity type mapping for the Client entity using the specified builder.
    /// </summary>
    /// <remarks>This method defines table mapping, property constraints, indexes, relationships, and owned
    /// types for the Client entity. It should be called within the Entity Framework Core model configuration process,
    /// typically in an implementation of IEntityTypeConfiguration<Client>.</remarks>
    /// <param name="b">The builder used to configure the Client entity type and its properties.</param>
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.ToTable("Clients");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.PartnerId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.TradeName).HasMaxLength(160).IsRequired();
        b.Property(x => x.LegalName).HasMaxLength(160).IsRequired();

        var cnpjConverter =  new ValueConverter<CnpjValueObject, string>(
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
                .HasMaxLength(14)
                .IsRequired();

        cnpjProp.Metadata.SetValueComparer(cnpjComparer);

        b.Property(x => x.StateRegistration).HasMaxLength(50);
        b.Property(x => x.MunicipalRegistration).HasMaxLength(50);
        b.Property(x => x.BillingDueDay).HasColumnType("smallint");
        b.Property(x => x.Status).IsRequired();

        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasIndex(x => x.PartnerId).HasDatabaseName("IX_Clients_PartnerId");
        b.HasIndex(x => x.Cnpj).IsUnique().HasDatabaseName("IX_Clients_Cnpj");

        b.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(160).IsRequired();
            a.Property(p => p.Number).HasColumnName("Number").IsRequired();
            a.Property(p => p.City).HasColumnName("City").HasMaxLength(80).IsRequired();
            a.Property(p => p.State).HasColumnName("State").HasMaxLength(2).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(10).IsRequired();
        });

        b.HasOne(x => x.Partner)
         .WithMany(x => x.Clients)
         .HasForeignKey(x => x.PartnerId)
         .HasConstraintName("FK_Partners_Clients")
         .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(c => c.Contacts)
           .WithOne(ct => ct.Client)
           .HasForeignKey(ct => ct.ClientId)
           .HasConstraintName("FK_Contacts_Client")
           .OnDelete(DeleteBehavior.NoAction);
    }
}