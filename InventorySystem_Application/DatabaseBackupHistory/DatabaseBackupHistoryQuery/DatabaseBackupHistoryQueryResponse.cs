namespace InventorySystem_Application.DatabaseBackupHistoryCommand.DatabaseBackupHistoryQuery;

public record DatabaseBackupHistoryQueryResponse(int BackupId, DateTime BackupDate, string BackupStatus,
    bool IsActive, string ErrorMessage, string CreatedBy);