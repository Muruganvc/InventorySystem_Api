namespace InventorySystem_Domain;

public class Backup
{
    public int BackupId { get; set; }
    public DateTime BackupDate { get; set; } = DateTime.UtcNow;
    public string BackupStatus { get; set; } = "SUCCESS"; // or "FAILED"
    public bool IsActive { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatedBy { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public static Backup Create(string backupStatus, int createdBy, bool isActive, string errorMessage)
    {
        return new Backup
        {
            BackupStatus = backupStatus,
            BackupDate = DateTime.UtcNow,
            ErrorMessage = errorMessage,
            IsActive = isActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
        };
    }
}