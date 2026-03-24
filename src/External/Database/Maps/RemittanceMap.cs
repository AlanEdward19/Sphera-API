using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        
        b.Property(x => x.ConfigurationId).HasColumnType("uniqueidentifier");
        b.HasIndex(x => x.ConfigurationId).HasDatabaseName("IX_Remittances_ConfigurationId");
        b.HasOne(x => x.Configuration)
            .WithMany()
            .HasForeignKey(x => x.ConfigurationId)
            .HasConstraintName("FK_Remittances_BilletConfigurations")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
