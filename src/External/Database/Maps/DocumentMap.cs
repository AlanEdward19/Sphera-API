using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Documents;

namespace Sphera.API.External.Database.Maps;

/// <summary>
/// Configures the entity mapping for the Document type within the Entity Framework Core model.
/// </summary>
/// <remarks>This configuration defines table mappings, property constraints, indexes, owned types, and
/// relationships for the Document entity. It is typically used within the OnModelCreating method of a DbContext to
/// ensure the Document entity is correctly mapped to the underlying database schema.</remarks>
public class DocumentMap : IEntityTypeConfiguration<Document>
{
    /// <summary>
    /// Configures the entity type mapping for the Document entity.
    /// </summary>
    /// <remarks>This method defines the table mapping, keys, property configurations, indexes, owned types,
    /// and relationships for the Document entity. It should be called within the Entity Framework Core model
    /// configuration process.</remarks>
    /// <param name="b">The builder used to configure the Document entity type.</param>
    public void Configure(EntityTypeBuilder<Document> b)
    {
        b.ToTable("Documents");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWID()");

        b.Property(x => x.ClientId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.ServiceId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.ResponsibleId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.IssueDate).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.DueDate).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.Notes).HasMaxLength(255);
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasIndex(x => new { x.ClientId, x.ServiceId }).HasDatabaseName("IX_Documents_Client_Service");
        b.HasIndex(x => x.DueDate).HasDatabaseName("IX_Documents_DueDate");
        
        b.OwnsOne(x => x.File, fm =>
        {
            fm.Property(f => f.FileName).HasColumnName("FileName").HasMaxLength(260).IsRequired();
            fm.Property(f => f.Size).HasColumnName("FileSize").HasColumnType("bigint");
            fm.Property(f => f.ContentType).HasColumnName("ContentType").HasMaxLength(100);
            fm.Property(f => f.BlobUri).HasColumnName("BlobUri").HasMaxLength(500);
        });

        b.HasOne(x => x.Client)
         .WithMany()
         .HasForeignKey(x => x.ClientId)
         .OnDelete(DeleteBehavior.Restrict)
         .HasConstraintName("FK_Documents_Client");

        b.HasOne(x => x.Service)
         .WithMany()
         .HasForeignKey(x => x.ServiceId)
         .OnDelete(DeleteBehavior.Restrict)
         .HasConstraintName("FK_Documents_Service");
    }
}