using Microsoft.EntityFrameworkCore;
using Sphera.API.Auditory;
using Sphera.API.Clients;
using Sphera.API.Contacts;
using Sphera.API.Documents;
using Sphera.API.External.Database.Maps;
using Sphera.API.Partners;
using Sphera.API.Roles;
using Sphera.API.Schedules;
using Sphera.API.Services;
using Sphera.API.Shared;
using Sphera.API.Shared.Utils;
using Sphera.API.Users;

namespace Sphera.API.External.Database;

public class SpheraDbContext(DbContextOptions<SpheraDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    public DbSet<Partner> Partners { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<AuditEntry> AuditEntries { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ScheduleEvent> ScheduleEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.ApplyConfiguration(new PartnerMap());
        modelBuilder.ApplyConfiguration(new ClientMap());
        modelBuilder.ApplyConfiguration(new ServiceMap());
        modelBuilder.ApplyConfiguration(new DocumentMap());
        modelBuilder.ApplyConfiguration(new AuditEntryMap());
        modelBuilder.ApplyConfiguration(new ContactMap());
        modelBuilder.ApplyConfiguration(new RoleMap());
        modelBuilder.ApplyConfiguration(new UserMap());

        #region Seed

        var seedDate = new DateTime(2025, 11, 26);

        modelBuilder.Entity<Role>().HasData(
            new Role (1, "Administrador",seedDate),
            new Role (2, "Gestor",seedDate),
            new Role (3, "Financeiro",seedDate)
        );

        #endregion

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var user = httpContextAccessor.HttpContext!.User;
        var clientIp = string.IsNullOrWhiteSpace(httpContextAccessor.HttpContext!.GetClientIp())
            ? "unknown"
            : httpContextAccessor.HttpContext!.GetClientIp();
        ChangeTracker.DetectChanges();
        
        if (user?.Identity?.IsAuthenticated == true)
        {
            var auditEntries = AuditHelper.CreateAuditEntries(ChangeTracker, user.GetUserId(), clientIp ?? "unknown");
            if (auditEntries.Any())
                AuditEntries.AddRange(auditEntries);
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // bubble up or wrap into a domain-specific concurrency exception
            throw;
        }
    }
}