using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.Billets;
using Sphera.API.Billing.Remittances;

namespace Sphera.API.External.Database.Maps;

public class RemittanceMap : IEntityTypeConfiguration<Remittance>
{
    public void Configure(EntityTypeBuilder<Remittance> b)
    {
        b.ToTable("Remittances");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.FileName)
            .HasMaxLength(260)
            .IsRequired();
        b.Property(x => x.IsSubmitted)
            .IsRequired();

        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
    }
}
