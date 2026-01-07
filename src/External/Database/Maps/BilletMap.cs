using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.Billets;
using Sphera.API.Billing.Remittances;

namespace Sphera.API.External.Database.Maps;

public class BilletMap : IEntityTypeConfiguration<Billet>
{
    public void Configure(EntityTypeBuilder<Billet> b)
    {
        b.ToTable("Billets");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

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

        b.Property(x => x.InstallmentId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        b.Property(x => x.ConfigurationId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        b.Property(x => x.ClientId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.HasIndex(x => x.InstallmentId).HasDatabaseName("IX_Billets_InstallmentId");

        b.HasOne(x => x.Installment)
            .WithMany()
            .HasForeignKey(x => x.InstallmentId)
            .HasConstraintName("FK_Billets_InvoiceInstallments")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Configuration)
            .WithMany()
            .HasForeignKey(x => x.ConfigurationId)
            .HasConstraintName("FK_Billets_BilletConfigurations")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_Billets_Clients")
            .OnDelete(DeleteBehavior.Restrict);

        b.Property<Guid?>("RemittanceId").HasColumnType("uniqueidentifier");
        b.HasIndex("RemittanceId").HasDatabaseName("IX_Billets_RemittanceId");
        b.HasOne<Remittance>()
            .WithMany(r => r.Billets)
            .HasForeignKey("RemittanceId")
            .HasConstraintName("FK_Billets_Remittances")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
