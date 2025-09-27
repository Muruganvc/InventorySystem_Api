using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.DatabaseBackupHistoryCommand.DatabaseBackupHistoryQuery;

internal sealed class DatabaseBackupHistoryQueryHandler
     : IRequestHandler<DatabaseBackupHistoryQuery, IResult<IReadOnlyList<DatabaseBackupHistoryQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Backup> _backupRepository;

    public DatabaseBackupHistoryQueryHandler(IRepository<InventorySystem_Domain.Backup> backupRepository)
    {
        _backupRepository = backupRepository;
    }
    public async Task<IResult<IReadOnlyList<DatabaseBackupHistoryQueryResponse>>> Handle(DatabaseBackupHistoryQuery request, CancellationToken cancellationToken)
    {
        var backups = await _backupRepository.Table
            .AsNoTracking()
            .Select(b => new DatabaseBackupHistoryQueryResponse(
                b.BackupId,
                b.BackupDate,
                b.BackupStatus,
                b.IsActive,
                b.ErrorMessage ?? string.Empty,
                b.CreatedByUser.UserName
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<DatabaseBackupHistoryQueryResponse>>.Success(
            backups.OrderBy(b => b.BackupDate).ToList()
        );
    }
}
