using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sphera.API.Auditory;

namespace Sphera.API.Shared;

public static class AuditHelper
{
    public static List<AuditEntry> CreateAuditEntries(ChangeTracker changeTracker, Guid actorId, string requestIp)
    {
        var result = new List<AuditEntry>();

        foreach (var entry in changeTracker.Entries().Where(e => e.Entity != null && !IsAuditEntry(e)))
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) continue;

            var entityType = entry.Entity.GetType().Name;
            Guid? entityId = TryGetPrimaryKey(entry);

            var action = entry.State.ToString(); // Added/Modified/Deleted
            
            string? entityName = ResolveEntityName(entry);

            var audit = new AuditEntry(actorId, action, entityType,
                entityId, requestIp, entityName);

            result.Add(audit);
        }

        return result;
    }

    private static bool IsAuditEntry(EntityEntry entry) => entry.Entity is AuditEntry;

    private static Guid? TryGetPrimaryKey(EntityEntry entry)
    {
        var pk = entry.Metadata.FindPrimaryKey();
        if (pk == null) return null;

        // support single PKs only for simplicity
        var pkProp = pk.Properties.FirstOrDefault();
        if (pkProp == null) return null;

        var val = entry.Property(pkProp.Name).CurrentValue ?? entry.Property(pkProp.Name).OriginalValue;
        if (val == null) return null;

        if (val is Guid g) return g;
        if (Guid.TryParse(val.ToString(), out var parsed)) return parsed;

        // if it's long/bigint, convert to Guid.Empty marker and let consumer interpret
        return null;
    }

    private static string? ResolveEntityName(EntityEntry entry)
    {
        try
        {
            var typeName = entry.Entity.GetType().Name;

            switch (typeName)
            {
                case "Client":
                    return GetStringProperty(entry, "LegalName") ?? GetStringProperty(entry, "TradeName");
                case "Partner":
                    return GetStringProperty(entry, "LegalName") ?? "";
                case "Service":
                    return GetStringProperty(entry, "Name");
                case "User":
                    return GetStringProperty(entry, "Name") ?? GetStringProperty(entry, "Email");
                case "Document":
                    return GetStringProperty(entry, "FileName") ?? GetPathIfDocument(entry);
                case "ScheduleEvent":
                    var occurred = entry.Property("OccurredAt").CurrentValue ?? entry.Property("OccurredAt").OriginalValue;
                    if (occurred is DateTime dt) return dt.ToString(System.Globalization.CultureInfo.CurrentCulture);
                    return occurred?.ToString();
                default:
                    return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private static string? GetStringProperty(EntityEntry entry, string propertyName)
    {
        var prop = entry.Properties.FirstOrDefault(p => string.Equals(p.Metadata.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
        var val = prop?.CurrentValue ?? prop?.OriginalValue;
        
        //gets string value, if the length is greater than 250, it will be truncated and added ...
        if (val is string strVal)
        {
            if (strVal.Length > 250)
                return strVal.Substring(0, 247) + "...";
            
            return strVal;
        }
        
        return "";
    }

    private static string? GetPathIfDocument(EntityEntry entry)
    {
        if (entry.Entity.GetType().Name == "Document")
        {
            // tenta extrair Owned File BlobUri
            var blobUriProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name.EndsWith("BlobUri", StringComparison.InvariantCultureIgnoreCase));
            var val = blobUriProp?.CurrentValue ?? blobUriProp?.OriginalValue;
            return val?.ToString();
        }
        return null;
    }

    private static string? GetFolderIfDocument(EntityEntry entry)
    {
        if (entry.Entity.GetType().Name == "Document")
        {
            // Podem mapear container/virtual folder conforme seu padrão (ex.: extrair do BlobUri)
            var blobUriProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name.EndsWith("BlobUri", StringComparison.InvariantCultureIgnoreCase));
            var val = blobUriProp?.CurrentValue ?? blobUriProp?.OriginalValue;
            if (val == null) return null;
            try
            {
                var uri = new Uri(val.ToString());
                var segments = uri.AbsolutePath.Trim('/').Split('/');
                if (segments.Length >= 2) return segments[1]; // container + folder... heurística
            }
            catch { }
        }
        return null;
    }
}
