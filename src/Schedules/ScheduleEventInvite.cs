using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sphera.API.Users;

namespace Sphera.API.Schedules;

public sealed class ScheduleEventInvite
{
    [Key]
    public Guid Id { get; private set; }

    public Guid ScheduleEventId { get; private set; }

    public Guid InvitedUserId { get; private set; }

    [ForeignKey(nameof(InvitedUserId))]
    public User? InvitedUser { get; private set; }

    [ForeignKey(nameof(ScheduleEventId))]
    public ScheduleEvent? ScheduleEvent { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private ScheduleEventInvite() { }

    public ScheduleEventInvite(Guid scheduleEventId, Guid invitedUserId)
    {
        Id = Guid.NewGuid();
        ScheduleEventId = scheduleEventId;
        InvitedUserId = invitedUserId;
        CreatedAt = DateTime.UtcNow;
    }
}

