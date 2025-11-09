using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Services;

namespace Sphera.API.External.Database.Maps;

/// <summary>
/// Configures the entity mapping for the Service type in the Entity Framework model.
/// </summary>
/// <remarks>This class defines how the Service entity is mapped to the database schema, including table name,
/// primary key, property configurations, and indexes. It is typically used by Entity Framework Core during model
/// creation to apply custom configuration for the Service entity.</remarks>
public class ServiceMap : IEntityTypeConfiguration<Service>
{
    /// <summary>
    /// Configures the entity type mapping for the Service entity.
    /// </summary>
    /// <remarks>This method defines the table mapping, primary key, property configurations, and indexes for
    /// the Service entity. It should be called within the Entity Framework Core model configuration process, typically
    /// in the OnModelCreating method of your DbContext.</remarks>
    /// <param name="b">The builder used to configure the Service entity type.</param>
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.ToTable("Services");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWID()");
        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
        b.Property(x => x.DueDate).HasColumnType("datetime2");
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasIndex(x => x.Code).IsUnique().HasDatabaseName("IX_Services_Code");
    }
}