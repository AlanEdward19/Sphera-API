using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Auditory;

namespace Sphera.API.External.Database.Maps;

/// <summary>
/// Provides configuration for the AuditEntry entity type to define its mapping to the database schema using Entity
/// Framework Core.
/// </summary>
/// <remarks>This class is typically used within the OnModelCreating method to configure table names, property
/// mappings, and indexes for the AuditEntry entity. It ensures that the entity is correctly mapped to the underlying
/// database structure according to application requirements.</remarks>
public class AuditEntryMap : IEntityTypeConfiguration<AuditEntry>
{
    /// <summary>
    /// Configures the entity mapping for the AuditEntry type using the provided EntityTypeBuilder.
    /// </summary>
    /// <remarks>This method defines table mapping, property configurations, and indexes for the AuditEntry
    /// entity. It should be called from the OnModelCreating method when setting up the Entity Framework Core
    /// model.</remarks>
    /// <param name="b">The builder used to configure the AuditEntry entity type and its properties.</param>
    public void Configure(EntityTypeBuilder<AuditEntry> b)
    {
        b.ToTable("AuditEntries");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
        b.Property(x => x.OccurredAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.ActorId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.Action).HasMaxLength(80).IsRequired();
        b.Property(x => x.EntityType).HasMaxLength(120).IsRequired();
        b.Property(x => x.EntityId).HasColumnType("uniqueidentifier");
        b.Property(x => x.RequestIp).HasMaxLength(45).IsRequired();

        b.HasIndex(x => new { x.EntityType, x.EntityId }).HasDatabaseName("IX_Audit_Entity");
        b.HasIndex(x => new { x.ActorId, x.OccurredAt }).HasDatabaseName("IX_Audit_Actor");
    }
}