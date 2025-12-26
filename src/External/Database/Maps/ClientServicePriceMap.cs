using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Billing.ClientServicePrices;

namespace Sphera.API.External.Database.Maps;

public class ClientServicePriceMap : IEntityTypeConfiguration<ClientServicePrice>
{
    public void Configure(EntityTypeBuilder<ClientServicePrice> b)
    {
        b.ToTable("ClientServicePrices");
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

        b.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        b.Property(x => x.StartDate)
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(x => x.EndDate)
            .HasColumnType("datetime2");

        b.Property(x => x.IsActive)
            .IsRequired();

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

        b.HasIndex(x => new { x.ClientId, x.ServiceId, x.StartDate })
            .HasDatabaseName("IX_ClientServicePrices_Client_Service_StartDate");

        b.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ClientServicePrices_Clients")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_ClientServicePrices_Services")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
