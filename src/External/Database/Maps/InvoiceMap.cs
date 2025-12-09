using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.Invoices;

namespace Sphera.API.External.Database.Maps;

public class InvoiceMap : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    {
        b.ToTable("Invoices");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.ClientId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.PeriodStart)
            .HasColumnType("date")
            .IsRequired();

        b.Property(x => x.PeriodEnd)
            .HasColumnType("date")
            .IsRequired();

        b.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>() // enum -> int
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(x => x.CreatedBy)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.ClosedAt)
            .HasColumnType("datetime2");

        b.Property(x => x.ClosedBy)
            .HasColumnType("uniqueidentifier");

        b.HasIndex(x => x.ClientId)
            .HasDatabaseName("IX_Invoices_ClientId");

        b.HasIndex(x => new { x.PeriodStart, x.PeriodEnd })
            .HasDatabaseName("IX_Invoices_Period");

        b.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_Invoices_Clients")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Items)
            .WithOne(i => i.Invoice)
            .HasForeignKey(i => i.InvoiceId)
            .HasConstraintName("FK_InvoiceItems_Invoices")
            .OnDelete(DeleteBehavior.Cascade);
    }
}