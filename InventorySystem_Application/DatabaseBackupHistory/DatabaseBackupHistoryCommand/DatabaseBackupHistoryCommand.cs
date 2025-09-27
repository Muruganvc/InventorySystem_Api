using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.DatabaseBackupHistory.DatabaseBackupHistoryCommand;
public record DatabaseBackupHistoryCommand(string BackupStatus, bool IsActive, string ErrorMessage) : IRequest<IResult<bool>>;

