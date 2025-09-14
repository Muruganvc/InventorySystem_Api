using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using InventorySystem_Infrastructure.TableConfiguration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InventorySystem_Infrastructure;
public class InventorySystemDbContext : DbContext
{
    private readonly IUserInfo _userInfo;
    public InventorySystemDbContext(DbContextOptions<InventorySystemDbContext> options, IUserInfo userInfo)
       : base(options)
    {
        _userInfo = userInfo;
    }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChangesAsync(auditEntries, cancellationToken);
        return result;
    }

    private async Task OnAfterSaveChangesAsync(List<AuditLog> auditLogs, CancellationToken cancellationToken)
    {
        if (auditLogs.Any())
        {
            AuditLogs.AddRange(auditLogs);
            await base.SaveChangesAsync(cancellationToken);
        }
    }

    private List<AuditLog> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditLogs = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            if (entry.Entity.GetType().Name == "FileDataBackup") continue;

            var auditLog = new AuditLog
            {
                TableName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                ChangedAt = DateTime.UtcNow,
                ChangedBy = _userInfo.UserName
            };

            var keyValues = entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey())
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

            auditLog.KeyValues = JsonSerializer.Serialize(keyValues);

            if (entry.State == EntityState.Modified)
            {
                var excludedProperties = new[] { "ProfileImage", "QcCode" };

                var modifiedProperties = entry.Properties
                    .Where(p => p.IsModified &&
                                !excludedProperties.Contains(p.Metadata.Name) &&
                                p.OriginalValue?.ToString() != p.CurrentValue?.ToString())
                    .ToList();

                if (modifiedProperties.Count == 0)
                    continue;

                var oldValues = modifiedProperties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                var newValues = modifiedProperties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

                auditLog.OldValues = JsonSerializer.Serialize(oldValues);
                auditLog.NewValues = JsonSerializer.Serialize(newValues);
            }

            else if (entry.State == EntityState.Added)
            {
                var newValues = entry.CurrentValues.Properties
                    .ToDictionary(p => p.Name, p => entry.CurrentValues[p]);

                auditLog.NewValues = JsonSerializer.Serialize(newValues);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var oldValues = entry.OriginalValues.Properties
                    .ToDictionary(p => p.Name, p => entry.OriginalValues[p]);

                auditLog.OldValues = JsonSerializer.Serialize(oldValues);
            }

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryCompanyInfoConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new UserMenuPermissionConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
    }
}
