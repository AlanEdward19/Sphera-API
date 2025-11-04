using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Auditory;

namespace Sphera.API.External.Database.Maps;

public class AuditEntryMap : IEntityTypeConfiguration<AuditEntry>
{
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
        b.Property(x => x.Path).HasMaxLength(500);
        b.Property(x => x.Folder).HasMaxLength(260);
        b.Property(x => x.CorrelationId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.RequestIp).HasMaxLength(45).IsRequired();

        b.HasIndex(x => new { x.EntityType, x.EntityId }).HasDatabaseName("IX_Audit_Entity");
        b.HasIndex(x => new { x.ActorId, x.OccurredAt }).HasDatabaseName("IX_Audit_Actor");
        b.HasIndex(x => x.CorrelationId).HasDatabaseName("IX_Audit_Correlation");
    }
}