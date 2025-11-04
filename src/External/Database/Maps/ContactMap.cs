using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Shared.Contacts;

namespace Sphera.API.External.Database.Maps;

public class ContactMap : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> b)
    {
        b.ToTable("Contacts");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.Type)
            .IsRequired();

        b.Property(x => x.Role)
            .IsRequired();

        b.Property(x => x.Value)
            .HasMaxLength(160)
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

        b.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        b.HasOne(x => x.Partner)
            .WithMany(p => p.Contacts)
            .HasForeignKey(x => x.PartnerId)
            .HasConstraintName("FK_Contacts_Partner")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Client)
            .WithMany(c => c.Contacts)
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_Contacts_Client")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.PartnerId).HasDatabaseName("IX_Contacts_PartnerId");
        b.HasIndex(x => x.ClientId).HasDatabaseName("IX_Contacts_ClientId");
        b.HasIndex(x => x.Role).HasDatabaseName("IX_Contacts_Role");
    }
}
