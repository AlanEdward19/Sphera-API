using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.Invoices;

namespace Sphera.API.External.Database.Maps;

public class InvoiceInstallmentMap : IEntityTypeConfiguration<InvoiceInstallment>
{
    public void Configure(EntityTypeBuilder<InvoiceInstallment> b)
    {
        b.ToTable("InvoiceInstallment");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.InvoiceId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.Number)
            .IsRequired();

        b.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.DueDate)
            .HasColumnType("date")
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        b.HasIndex(x => x.InvoiceId).HasDatabaseName("IX_InvoiceInstallments_InvoiceId");

        b.HasOne(x => x.Invoice)
            .WithMany(i => i.Installments)
            .HasForeignKey(x => x.InvoiceId)
            .HasConstraintName("FK_InvoiceInstallments_Invoices")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

