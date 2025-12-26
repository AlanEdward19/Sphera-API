using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.BillingEntries;

namespace Sphera.API.External.Database.Maps;

public class BillingEntryMap : IEntityTypeConfiguration<BillingEntry>
{
    public void Configure(EntityTypeBuilder<BillingEntry> b)
    {
        b.ToTable("BillingEntries");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.ClientId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.ServiceId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.ServiceDate)
            .HasColumnType("date")
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        b.Property(x => x.IsBillable)
            .IsRequired();

        b.Property(x => x.Notes)
            .HasMaxLength(500);

        b.Property(x => x.InvoiceId)
            .HasColumnType("uniqueidentifier");

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

        b.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        b.HasIndex(x => x.ClientId)
            .HasDatabaseName("IX_BillingEntries_ClientId");

        b.HasIndex(x => x.ServiceId)
            .HasDatabaseName("IX_BillingEntries_ServiceId");

        b.HasIndex(x => new { x.ServiceDate, x.IsBillable, x.InvoiceId })
            .HasDatabaseName("IX_BillingEntries_Period_Billable_Invoice");

        b.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_BillingEntries_Clients")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_BillingEntries_Services")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
