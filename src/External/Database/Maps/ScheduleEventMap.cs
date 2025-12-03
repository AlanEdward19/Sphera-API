using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Schedules;

namespace Sphera.API.External.Database.Maps;

public class ScheduleEventMap : IEntityTypeConfiguration<ScheduleEvent>
{
    public void Configure(EntityTypeBuilder<ScheduleEvent> b)
    {
        b.ToTable("ScheduleEvents");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.OccurredAt)
            .HasColumnType("datetime2")
            .IsRequired();

        b.Property(x => x.UserId)
            .HasColumnType("uniqueidentifier");

        b.Property(x => x.ClientId)
            .HasColumnType("uniqueidentifier");

        b.Property(x => x.Notes)
            .HasMaxLength(500);

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

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_ScheduleEvents_UserId");

        b.HasIndex(x => x.ClientId)
            .HasDatabaseName("IX_ScheduleEvents_ClientId");

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_ScheduleEvents_User")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        b.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .HasConstraintName("FK_ScheduleEvents_Client")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
