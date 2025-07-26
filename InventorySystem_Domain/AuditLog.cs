namespace InventorySystem_Domain;
public class AuditLog
{
    public int AuditLogId { get; set; }
    public string? TableName { get; set; }
    public string? Action { get; set; }
    public string? KeyValues { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}