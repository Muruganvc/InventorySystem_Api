using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.DatabaseBackupHistory.DatabaseBackupHistoryCommand;

internal sealed class DatabaseBackupHistoryCommandHandler
    : IRequestHandler<DatabaseBackupHistoryCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Backup> _backuppRepository;
    private readonly IUserInfo _userInfo;
    public DatabaseBackupHistoryCommandHandler(IUnitOfWork unitOfWork, IRepository<InventorySystem_Domain.Backup> backuppRepository,
        IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _backuppRepository = backuppRepository;
        _userInfo = userInfo;
    }
    public async Task<IResult<bool>> Handle(DatabaseBackupHistoryCommand request, CancellationToken cancellationToken)
    {
        var backup = InventorySystem_Domain.Backup.Create(request.BackupStatus, _userInfo.UserId, request.IsActive, request.ErrorMessage);

        var IsBackupSuccess = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _backuppRepository.AddAsync(backup);
            await _unitOfWork.SaveAsync();
            return backup.BackupId > 0;
        }, cancellationToken);

        return Result<bool>.Success(IsBackupSuccess);
    }
}
