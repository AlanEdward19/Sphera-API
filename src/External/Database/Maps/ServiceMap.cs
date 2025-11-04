using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Services;

namespace Sphera.API.External.Database.Maps;

public class ServiceMap : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.ToTable("Services");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWID()");
        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
        b.Property(x => x.DefaultDueInDays).HasColumnType("smallint").IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasIndex(x => x.Code).IsUnique().HasDatabaseName("IX_Services_Code");
    }
}