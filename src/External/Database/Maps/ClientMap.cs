using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Clients;
using Sphera.API.Partners;

namespace Sphera.API.External.Database.Maps;

public class ClientMap : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.ToTable("Clients");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWID()");
        b.Property(x => x.PartnerId).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.TradeName).HasMaxLength(160).IsRequired();
        b.Property(x => x.LegalName).HasMaxLength(160).IsRequired();
        b.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
        b.Property(x => x.StateRegistration).HasMaxLength(50);
        b.Property(x => x.MunicipalRegistration).HasMaxLength(50);
        b.Property(x => x.BillingDueDay).HasColumnType("smallint");
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.FinancialEmail).HasMaxLength(160).IsRequired();
        b.Property(x => x.FinancialPhone).HasMaxLength(40).IsRequired();

        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).HasColumnType("uniqueidentifier").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        b.Property(x => x.UpdatedBy).HasColumnType("uniqueidentifier");
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasIndex(x => x.PartnerId).HasDatabaseName("IX_Clients_PartnerId");
        b.HasIndex(x => x.Cnpj).IsUnique().HasDatabaseName("IX_Clients_Cnpj");
        
        b.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(160).IsRequired();
            a.Property(p => p.Number).HasColumnName("Number").IsRequired();
            a.Property(p => p.City).HasColumnName("City").HasMaxLength(80).IsRequired();
            a.Property(p => p.State).HasColumnName("State").HasMaxLength(2).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(10).IsRequired();
        });
        
        b.HasOne<Partner>().WithMany()
         .HasForeignKey(x => x.PartnerId)
         .OnDelete(DeleteBehavior.Restrict)
         .HasConstraintName("FK_Clients_Partner");
    }
}