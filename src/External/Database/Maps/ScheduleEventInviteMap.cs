using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sphera.API.Schedules;

namespace Sphera.API.External.Database.Maps;

public class ScheduleEventInviteMap : IEntityTypeConfiguration<ScheduleEventInvite>
{
    public void Configure(EntityTypeBuilder<ScheduleEventInvite> b)
    {
        b.ToTable("ScheduleEventInvites");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWID()");

        b.Property(x => x.ScheduleEventId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.InvitedUserId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        b.HasOne(x => x.ScheduleEvent)
            .WithMany(s => s.InvitedUsers)
            .HasForeignKey(x => x.ScheduleEventId)
            .HasConstraintName("FK_ScheduleEventInvites_ScheduleEvent")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.InvitedUser)
            .WithMany()
            .HasForeignKey(x => x.InvitedUserId)
            .HasConstraintName("FK_ScheduleEventInvites_User")
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ScheduleEventId).HasDatabaseName("IX_ScheduleEventInvites_ScheduleEventId");
        b.HasIndex(x => x.InvitedUserId).HasDatabaseName("IX_ScheduleEventInvites_InvitedUserId");
    }
}

