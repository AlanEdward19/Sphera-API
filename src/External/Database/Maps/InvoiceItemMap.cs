using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.Invoices;

namespace Sphera.API.External.Database.Maps;

public class InvoiceItemMap : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> b)
    {
        b.ToTable("InvoiceItems");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.InvoiceId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.ServiceId)
            .HasColumnType("uniqueidentifier");

        b.Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        b.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.AdditionalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.IsAdditional)
            .IsRequired();

        b.HasIndex(x => x.InvoiceId)
            .HasDatabaseName("IX_InvoiceItems_InvoiceId");

        b.HasIndex(x => x.ServiceId)
            .HasDatabaseName("IX_InvoiceItems_ServiceId");

        b.HasOne(x => x.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(x => x.InvoiceId)
            .HasConstraintName("FK_InvoiceItems_Invoices")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_InvoiceItems_Services")
            .OnDelete(DeleteBehavior.Restrict);
    }
}