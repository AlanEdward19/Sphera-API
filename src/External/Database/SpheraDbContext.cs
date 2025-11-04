using Microsoft.EntityFrameworkCore;
using Sphera.API.Auditory;
using Sphera.API.Clients;
using Sphera.API.Contacts;
using Sphera.API.Database.Maps;
using Sphera.API.Documents;
using Sphera.API.Partners;
using Sphera.API.Services;
using Sphera.API.Shared;

namespace Sphera.API.External.Database;

public class SpheraDbContext(DbContextOptions<SpheraDbContext> options) : DbContext(options)
{
    public DbSet<Partner> Partners { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<AuditEntry> AuditEntries { get; set; }
    public DbSet<Contact> Contacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.ApplyConfiguration(new PartnerMap());
        modelBuilder.ApplyConfiguration(new ClientMap());
        modelBuilder.ApplyConfiguration(new ServiceMap());
        modelBuilder.ApplyConfiguration(new DocumentMap());
        modelBuilder.ApplyConfiguration(new AuditEntryMap());
        modelBuilder.ApplyConfiguration(new ContactMap());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = AuditHelper.CreateAuditEntries(ChangeTracker,  Guid.Empty,
            Guid.Empty, "unknown");

        if (auditEntries.Any())
            AuditEntries.AddRange(auditEntries);

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